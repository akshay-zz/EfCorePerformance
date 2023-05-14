using Microsoft.EntityFrameworkCore;

namespace EfCorePerformance
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<HumanBeing> Person { get; set; } = null!;
    }
}
