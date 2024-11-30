using Hangfire.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BatchOldMigratedController : ControllerBase
    {
        private readonly IBatchOldMigratedService _services;
        private readonly ILogger<BatchOldMigratedController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IRecurringJobManager _recurringJobManager;

        public BatchOldMigratedController(IBatchOldMigratedService services, ILogger<BatchOldMigratedController> logger, IConfiguration configuration, IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager)
        {
            _services = services;
            _logger = logger;
            _configuration = configuration;
            _backgroundJobClient = backgroundJobClient;
            _recurringJobManager = recurringJobManager;
        }

        /// <summary>
        /// Enqueues batch 12 process.
        /// </summary>
        /// <remarks>
        /// Start batch 12 process one time.
        /// </remarks>
        /// <returns>HTTP 200 OK if the job was enqueued successfully.</returns>
        [HttpGet]
        [Route("Enqueue")]
        public Task<IActionResult> BatchOldMigrated()
        {
            _backgroundJobClient.Enqueue<IBatchOldMigratedService>(x => x.DoWorkOldBatchProcess());
            Console.WriteLine("BatchOldMigrated Triggered.");
            return Task.FromResult<IActionResult>(Ok());
        }

        /// <summary>
        /// RecurringJob batch 12 process.
        /// </summary>
        /// <remarks>
        /// Start batch 12 process every 03:00:00 AM.
        /// </remarks>
        /// <returns>HTTP 200 OK if the job was recurring successfully.</returns>
        [HttpGet]
        [Route("RecurringJob")]
        public Task<IActionResult> BatchOldMigratedRecurringJob()
        {
            _recurringJobManager.AddOrUpdate<IBatchOldMigratedService>(
                "BatchOldMigratedRecurringJob", // Unique ID for the recurring job
                x => x.DoWorkOldBatchProcess(),
                Cron.Minutely // or any other Cron expression if want recurring job every 03.00AM use "0 3 * * *"
            );
            return Task.FromResult<IActionResult>(Ok());
        }
    }
}