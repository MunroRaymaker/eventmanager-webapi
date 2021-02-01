using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace EventManager.WebAPI.Model
{
    /// <summary>
    ///     Represents an in-memory repository for maintaining event jobs
    /// </summary>
    public class Repository : IRepository
    {
        private readonly ILogger<Repository> logger;

        private readonly JobContext db = new JobContext();

        public Repository(ILogger<Repository> logger)
        {
            this.logger = logger;
        }

        public int Upsert(EventJob job)
        {
            try
            {
                var item = this.db.Jobs.Find(job.Id);

                // New job
                if (item == null)
                {
                    //job.Id = GetJobs().Any() ? GetJobs().Max(x => x.Id) + 1 : 1;
                    job.TimeStamp = DateTime.UtcNow;

                    this.db.Jobs.Add(job);
                    this.db.SaveChanges();

                    return job.Id;
                }

                // Update existing
                item.Data = job.Data;
                item.Name = job.Name;
                item.UserName = job.UserName;

                this.db.Jobs.Update(item);
                this.db.SaveChanges();

                return item.Id;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Unable to upsert job.");
                return -1;
            }
        }

        public IEnumerable<EventJob> GetJobs()
        {
            return this.db.Jobs.ToArray();
        }

        public EventJob GetJob(int id)
        {
            return this.db.Jobs.Find(id);
        }
    }
}