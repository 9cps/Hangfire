using Hangfire.Database;
using Hangfire.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Hangfire.Repositories
{
    public class HealthCheckRepository : IHealthCheckRepository
    {
        private readonly ILogger<HealthCheckRepository> _logger;
        private readonly ApplicationDbContext _context;

        public HealthCheckRepository(ILogger<HealthCheckRepository> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<HealthCheckResult> GetStatusDatabase()
        {
            try
            {
                // Get the database connection from the DbContext
                var connection = _context.Database.GetDbConnection();

                // Ensure the connection is open
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT 1";

                // Execute the command
                var result = await command.ExecuteScalarAsync();
                command.Dispose();

                // Check the result
                if (result != null)
                {
                    return HealthCheckResult.Healthy("Database is healthy.");
                }

                return HealthCheckResult.Unhealthy("Database query returned no result.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while checking database health.");
                return HealthCheckResult.Unhealthy("An error occurred while checking the database health.");
            }
        }
    }
}