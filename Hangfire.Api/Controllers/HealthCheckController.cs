using Hangfire.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Hangfire.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthCheckController : ControllerBase
    {
        private readonly IHealthCheckService _healthCheckService;
        private readonly ILogger<HealthCheckController> _logger;

        public HealthCheckController(IHealthCheckService healthCheckService, ILogger<HealthCheckController> logger)
        {
            _healthCheckService = healthCheckService;
            _logger = logger;
        }

        /// <summary>
        /// API check connection
        /// </summary>
        /// <remarks>
        /// API for check connection database.
        /// </remarks>
        [HttpGet("status")]
        public async Task<IActionResult> GetHealthStatus()
        {
            _logger.LogInformation("This is an informational message.");
            _logger.LogWarning("This is a warning message.");
            _logger.LogError("This is an error message.");
            var result = await _healthCheckService.GetStatusDatabase();
            return result.Status == HealthStatus.Healthy ? Ok(result) : StatusCode(500, result);
        }
    }
}
