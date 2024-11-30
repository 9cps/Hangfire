using Hangfire.Repositories.Interface;
using Hangfire.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Hangfire.Services
{
    public class HealthCheckService : IHealthCheckService
    {
        private readonly IHealthCheckRepository _repo;
        private readonly ILogger<HealthCheckService> _logger;
        private readonly IConfiguration _configuration;
        public HealthCheckService(IHealthCheckRepository repo, ILogger<HealthCheckService> logger, IConfiguration configuration)
        {
            _repo = repo;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<HealthCheckResult> GetStatusDatabase()
        {
            return await _repo.GetStatusDatabase();
        }
    }
}