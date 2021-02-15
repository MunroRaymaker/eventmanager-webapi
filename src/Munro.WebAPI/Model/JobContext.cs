using Microsoft.EntityFrameworkCore;

namespace EventManager.WebAPI.Model
{
    public class JobContext : DbContext
    {
        public JobContext()
        {
            
        }

        public JobContext(DbContextOptions<JobContext> options) : base(options)
        {
        }

        public DbSet<Job> JobItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("JobsList");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Data must be converted to a string to keep in storage
            var converter = new IntArrayToStringValueConverter();
            modelBuilder.Entity<Job>()
                .Property(e => e.Data)
                .HasConversion(converter);

            base.OnModelCreating(modelBuilder);
        }
    }
}