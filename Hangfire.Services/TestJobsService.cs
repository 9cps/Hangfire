using Hangfire.Repository.Interface;
using Hangfire.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hangfire.Services
{
    public class TestJobsService : ITestJobs
    {
        private readonly ITestJobsRepository _repo;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        public TestJobsService(ITestJobsRepository repo, ILogger logger, IConfiguration configuration)
        {
            _repo = repo;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<string> SendWelcomeEmail(string email, string name)
        {
            string result = $"Send email : {email}, Name : {name}, Datetime => {DateTime.Now}";
            Console.WriteLine(result);
            return result;
        }
    }
}