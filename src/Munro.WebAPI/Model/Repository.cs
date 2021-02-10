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
        
        // Dictionary does not use O(n) but implements a hashing algorithm to find data.
        private static IDictionary<int, EventJob> EventJobs { get; } = new Dictionary<int, EventJob>();
        private readonly object readLock = new object();

        public Repository(ILogger<Repository> logger)
        {
            this.logger = logger;
        }

        public int Upsert(EventJob job)
        {
            try
            {
                var item = GetJob(job.Id);

                // New job
                if (item == null)
                {
                    lock (readLock)
                    {
                        job.Id = GetJobs().Any() ? GetJobs().Max(x => x.Id) + 1 : 1;
                    }

                    job.TimeStamp = DateTime.UtcNow;
                    EventJobs.Add(job.Id, job);
                    return job.Id;
                }

                // Update existing
                item.Data = job.Data;
                item.Name = job.Name;
                item.UserName = job.UserName;

                return item.Id;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Unable to upsert job.");
                throw;
            }
        }

        public IEnumerable<EventJob> GetJobs()
        {
            return EventJobs.Values;
        }

        public EventJob GetJob(int id)
        {
            return EventJobs.TryGetValue(id, out var job) ? job : null;
        }
    }
}