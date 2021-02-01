using Microsoft.EntityFrameworkCore;

namespace EventManager.WebAPI.Model
{
    public class JobContext : DbContext
    {
        public DbSet<EventJob> Jobs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Data must be converted to a string to keep in storage
            var converter = new IntArrayToStringValueConverter();
            modelBuilder.Entity<EventJob>()
                .Property(e => e.Data) 
                .HasConversion(converter);

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=jobs.db");
        }
    }
}
