using Hangfire.Repositories.Interface;
using Hangfire.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace HealthCheckTests
{
    public class HealthCheckServiceTests
    {
        private readonly Mock<IHealthCheckRepository> _mockRepo;
        private readonly Mock<ILogger<HealthCheckService>> _mockLogger;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly HealthCheckService _service;

        public HealthCheckServiceTests()
        {
            _mockRepo = new Mock<IHealthCheckRepository>();
            _mockLogger = new Mock<ILogger<HealthCheckService>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _service = new HealthCheckService(_mockRepo.Object, _mockLogger.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task GetStatusDatabase_ReturnsExpectedResult()
        {
            // Arrange
            var expectedResult = HealthCheckResult.Healthy("Database is healthy");
            _mockRepo.Setup(repo => repo.GetStatusDatabase()).ReturnsAsync(expectedResult);

            // Act
            var result = await _service.GetStatusDatabase();

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}
