using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Hangfire.Repositories.Interface
{
    public interface IHealthCheckRepository
    {
        Task<HealthCheckResult> GetStatusDatabase();
    }
}
