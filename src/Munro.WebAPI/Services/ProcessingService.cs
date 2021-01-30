using System;
using EventManager.WebAPI.Model;
using System.Collections.Concurrent;
using System.Linq;
using EventManager.WebAPI.Controllers;
using EventManager.WebAPI.Extensions;
using Microsoft.Extensions.Logging;

namespace EventManager.WebAPI.Services
{
    public interface IProcessingService
    {

    }

    public class ProcessingService : IProcessingService
    {
        private readonly ILogger<EventManagerController> logger;

        public ProcessingService(ILogger<EventManagerController> logger)
        {
            this.logger = logger;

            // Add some fake data
            for (int i = 0; i < 100; i++)
            {
                Jobs.Enqueue(new EventJob
                {
                    Id = i,
                    IsCompleted = false,
                    Status = "Pending",
                    TimeStamp = DateTime.UtcNow,
                    Duration = 0,
                    Data = Enumerable.Range(1, 10).Shuffle().ToArray()
                });
            }
        }

        // This must be static to keep state across requests, since webapi is stateless.
        public static ConcurrentQueue<EventJob> Jobs { get; set; } = new ConcurrentQueue<EventJob>();
    }
}
