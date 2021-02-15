using System;
using System.Linq;

namespace EventManager.WebAPI.Model
{
    public static class DbInitializer
    {
        public static void Initialize(JobContext context)
        {
            context.Database.EnsureCreated();

            if (context.JobItems.Any()) return; // DB has been seeded

            var jobs = new[]
            {
                new Job{ Name = "job1", UserName = "Johnny", TimeStamp = DateTime.UtcNow, Data = new []{ 1, 2, 3 }},
                new Job{ Name = "job2", UserName = "Johnny", TimeStamp = DateTime.UtcNow, Data = new []{ 1, 2, 3 }},
                new Job{ Name = "job3", UserName = "Johnny", TimeStamp = DateTime.UtcNow, Data = new []{ 1, 2, 3 }},
            };

            foreach (var job in jobs)
            {
                job.Complete();
            }

            context.JobItems.AddRange(jobs);
            context.SaveChanges();
        }
    }
}
