


using Microsoft.EntityFrameworkCore;

namespace Hangfire.Database;

public class ApplicationDbContextFactory
{
    private readonly string _connectionString;
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public ApplicationDbContextFactory(DbContextOptions<ApplicationDbContext> options)
    {
        _options = options;
    }

    public ApplicationDbContextFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public ApplicationDbContext CreateDbContext()
    {
        if (!string.IsNullOrWhiteSpace(_connectionString))
        {
            return new ApplicationDbContext(_connectionString);
        }
        return new ApplicationDbContext(_options);
    }
}
