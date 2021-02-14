using Microsoft.EntityFrameworkCore;

namespace EventManager.WebAPI.Model
{
    public class JobContext : DbContext
    {
        public JobContext(DbContextOptions<JobContext> options) : base(options) { }

        private DbSet<Job> JobItems { get; set; }
    }
}
