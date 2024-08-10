using Microsoft.EntityFrameworkCore;

namespace Hangfire.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        { 
            
        }

    }
}
