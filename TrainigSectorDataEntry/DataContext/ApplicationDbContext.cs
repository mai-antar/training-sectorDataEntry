using Microsoft.EntityFrameworkCore;
using TrainigSectorDataEntry.Enities;

namespace TrainigSectorDataEntry.DataContext
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }

        // Add your DbSet properties here
        public DbSet<Employee> Employees { get; set; }
        
    }
}
