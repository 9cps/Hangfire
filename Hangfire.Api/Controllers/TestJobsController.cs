using Hangfire.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestJobsController : ControllerBase
    {
        private readonly ITestJobs _services;
        private readonly ILogger<TestJobsController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IRecurringJobManager _recurringJobManager;

        public TestJobsController(ITestJobs services, ILogger<TestJobsController> logger, IConfiguration configuration, IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager)
        {
            _services = services;
            _logger = logger;
            _configuration = configuration;
            _backgroundJobClient = backgroundJobClient;
            _recurringJobManager = recurringJobManager;
        }

        [HttpGet]
        [Route("TestJobs")]
        public async Task<IActionResult> TestJobs()
        {
            _backgroundJobClient.Enqueue<ITestJobs>(x => x.SendWelcomeEmail("mail@testjob.com", "fname"));
            Console.WriteLine("mail@testjob.com");
            return Ok();
        }

        [HttpGet]
        [Route("TestJobsRecurringJob")]
        public async Task<IActionResult> TestJobsRecurringJob()
        {
            _recurringJobManager.AddOrUpdate<ITestJobs>(
                "TestJobsRecurringJob", // Unique ID for the recurring job
                x => x.SendWelcomeEmail("mail@testjob.com", "fname"),
                Cron.Minutely // or any other Cron expression
            );
            Console.WriteLine("Recurring job scheduled for mail@testjob.com");
            return Ok();
        }
    }
}