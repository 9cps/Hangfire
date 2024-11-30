using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Hangfire.Services.Interface
{
    public interface IHealthCheckService
    {
        public Task<HealthCheckResult> GetStatusDatabase();
    }
}
