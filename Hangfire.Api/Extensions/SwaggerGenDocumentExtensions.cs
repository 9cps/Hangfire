using Hangfire.Shared.Helper;

namespace Hangfire.Api.Extensions
{
    public static class SwaggerGenDocumentExtensions
    {
        public static IApplicationBuilder UseSwaggerGenDocument(this IApplicationBuilder app, WebApplication apps, IWebHostEnvironment environment)
        {

            app.UseSwagger(o =>
            {
                o.SerializeAsV2 = false;
            });

            if (ConfigurationHelper.config["Environment"] != "PRD")
            {
                app.UseSwaggerUI();
            }

            return app;
        }

    }
}
