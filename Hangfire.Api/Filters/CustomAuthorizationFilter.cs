using Hangfire.Dashboard;

namespace Hangfire.Api.Filters
{
    public class CustomAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public CustomAuthorizationFilter(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public bool Authorize(DashboardContext context)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            // Check if the authentication cookie exists

            if (httpContext.Request.Cookies.ContainsKey("MyCookieAuthTDR") || Convert.ToBoolean(_configuration.GetSection("BypassAuthSetting").GetSection("Login").Value))
            {
                return true; // User is authorized
            }

            return false; // Not authorized
        }
    }
}
