using Hangfire;
using Hangfire.Api.Extensions;
using Hangfire.Api.Filters;
using Hangfire.Api.Middleware;
using Hangfire.Database;
using Hangfire.Models;
using Hangfire.Repositories;
using Hangfire.Repositories.Interface;
using Hangfire.Services;
using Hangfire.Services.Interface;
using Hangfire.Shared.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
ConfigurationHelper.Initialize(builder.Configuration);

var encryptionKey = builder.Configuration.GetValue<string>("EncryptionKey");
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")?.Decrypt(encryptionKey) ?? "";
var corsPolicy = "ApiCorsPolicy";
string[] corsOrigin = builder.Configuration.GetSection("AllowOrgin").Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicy,
        builder =>
        {
            builder
            .WithOrigins(corsOrigin)
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
    .AddJsonFile("seri-log.config.json")
    .Build())
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Add services to the container.
builder.Services.AddControllersWithViews(); // Ensure this is only declared once
builder.Services.AddRazorPages(); // Add this for Razor Pages

// Configure Hangfire Client
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    //.UseRecommendedSerializerSettings()
    .UseSqlServerStorage(connectionString)
    .UseSerializerSettings(new JsonSerializerSettings
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
    })
);

// Hangfire Server
builder.Services.AddHangfireServer();

// Register ApplicationDbContext with the DI container
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddSignalR();

// Add services to the container.
builder.Services.AddScoped<IBaseRepository, BaseRepository>();

builder.Services.AddScoped<IBatchOldMigratedService, BatchOldMigratedService>();
builder.Services.AddScoped<IBatchOldMigratedRepository, BatchOldMigratedRepository>();

builder.Services.AddScoped<IHealthCheckService, HealthCheckService>();
builder.Services.AddScoped<IHealthCheckRepository, HealthCheckRepository>();

builder.Services.AddScoped<IPdpaService, PdpaService>();
builder.Services.AddScoped<IPdpaRepository, PdpaRepository>();
builder.Services.AddScoped<ApplicationDbContextFactory>();
builder.Services.AddControllers();


builder.Services.Configure<PdpaConfig>(builder.Configuration.GetSection("PdpaConfig"));
builder.Services.Configure<BatchOldConfig>(builder.Configuration.GetSection("BatchOldConfig"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// Configure Health Checks
builder.Services.AddHealthChecks();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Hangfire API",
        Version = "v1",
        Description = "A sample API for Hangfire job management",
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

// Authentication
builder.Services.AddAuthentication("MyCookieAuthTDR")
    .AddCookie("MyCookieAuthTDR", options =>
    {
        options.Cookie.Name = "MyCookieAuthTDR";
        options.LoginPath = "/login"; // เส้นทางไปยังหน้า login
        // options.LogoutPath = "/logout"; // เส้นทางไปยังหน้า logout
        options.ExpireTimeSpan = TimeSpan.FromHours(1); // เวลา expiration ของคุกกี้
        options.SlidingExpiration = true; // ถ้าเปิดใช้งาน คุกกี้จะหมดอายุหลังจากไม่มีกิจกรรม
    });

// Register the custom authorization filter
builder.Services.AddSingleton<CustomAuthorizationFilter>();

var app = builder.Build();

app.UseRouting();
app.UseCors(corsPolicy);
//app.UseSerilogRequestLogging();

// Add authentication and authorization middleware
app.UseStaticFiles();
app.UseMiddleware<CookieAuthMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.UseDeveloperExceptionPage();

app.UseSwaggerGenDocument(app, builder.Environment);
app.UseHttpsRedirection();

app.MapControllers();
app.MapRazorPages();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { app.Services.GetRequiredService<CustomAuthorizationFilter>() },
    DashboardTitle = "TDR Scheduler",
    DarkModeEnabled = true
});
app.MapHealthChecks("/health");

var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
var options = new RecurringJobOptions
{
    TimeZone = timeZone,
};

//Add hangfire job
RecurringJob.AddOrUpdate<IPdpaService>(
    "RJ-UpdPdpaProcess",
   x => x.UpdPdpaProcess(),
   builder.Configuration.GetSection("PdpaConfig").GetSection("InBound").GetValue<string>("TimeScheduler"),
   options
);

RecurringJob.AddOrUpdate<IPdpaService>(
    "RJ-DldPdpaProcess",
    x => x.DldPdpaProcess(),
    builder.Configuration.GetSection("PdpaConfig").GetSection("OutBound").GetValue<string>("TimeScheduler"),
    options
);

app.Run();
