using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Hangfire.Api.Middleware
{
    public class CookieAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public CookieAuthMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                if (Convert.ToBoolean(_configuration.GetSection("BypassAuthSetting").GetSection("Api").Value))
                {
                    await _next(context);
                }
            }

            // ยกเว้น path "/login"
            if (!context.Request.Path.StartsWithSegments("/login"))
            {
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    // ตรวจสอบว่าเป็นการเรียก API ใช่หรือไม่
                    var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                    if (string.IsNullOrEmpty(token))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized; // ไม่มีโทเค็น
                        return;
                    }

                    var authBearer = await context.AuthenticateAsync("Bearer");
                    if (authBearer == null || !authBearer.Succeeded)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized; // โทเค็นไม่ถูกต้อง
                        return;
                    }

                    await _next(context);
                }

                if (Convert.ToBoolean(_configuration.GetSection("BypassAuthSetting").GetSection("Login").Value))
                {
                    if (!context.Request.Cookies.ContainsKey("MyCookieAuthTDR"))
                    {
                        var claims = new[]
                        {
                            new Claim(ClaimTypes.Name, "admin")
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuthTDR");
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = true, // ถ้าต้องการให้คุกกี้อยู่ตลอด
                            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1) // ตั้งเวลา expiration
                        };
                        await context.SignInAsync("MyCookieAuthTDR", new ClaimsPrincipal(claimsIdentity), authProperties);
                    }
                }
                else
                {
                    // เช็คคุกกี้ "MyCookieAuthTDR"
                    var authCookie = await context.AuthenticateAsync("MyCookieAuthTDR");
                    if (!context.Request.Cookies.ContainsKey("MyCookieAuthTDR"))
                    {
                        // ถ้าไม่มีคุกกี้ให้ redirect ไปที่ /login
                        context.Response.Redirect("/login");
                        return;
                    }

                    // เช็คเวลา expiration ของคุกกี้
                    if (!authCookie.Succeeded || authCookie.Properties.ExpiresUtc < DateTimeOffset.UtcNow)
                    {
                        context.Response.Cookies.Delete("MyCookieAuthTDR");

                        // ถ้าคุกกี้หมดอายุหรือไม่มี ให้ redirect ไปที่ /login
                        context.Response.Redirect("/login");
                        return;
                    }
                }
            }

            // ถ้ามีคุกกี้หรือไม่ใช่ path ที่ต้องการให้ผ่านไป
            await _next(context);
        }
    }

}
