using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EventManager.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventManagerController : ControllerBase
    {
        private readonly ILogger<EventManagerController> _logger;

        private readonly List<EventJob> Jobs;
        
        public EventManagerController(ILogger<EventManagerController> logger)
        {
            this._logger = logger;
            this.Jobs = new List<EventJob>();
            
            // Add some fake data
            for (int i = 0; i < 100; i++)
            {
                this.Jobs.Add(new EventJob
                {
                    Id = i,
                    IsCompleted = false,
                    Status = "Pending",
                    TimeStamp = DateTime.UtcNow,
                    Duration = TimeSpan.Zero,
                    Data = Enumerable.Range(1,100).ToArray()
                });
            }
        }

        [HttpGet]
        public IEnumerable<EventJob> Get()
        {
            return this.Jobs;
        }

        [HttpGet("byId/{id}", Name = "GetById")]
        public async Task<EventJob> GetById(int id)
        {
            return this.Jobs.FirstOrDefault(x => x.Id == id);
        }
        
        [HttpPost("AddJob", Name = "AddJob")]
        public EventJob AddJob([FromBody] int[] data)
        {
            var job = new EventJob
            {
                Id = this.Jobs.Any() ? this.Jobs.Max(x => x.Id) + 1 : 1,
                TimeStamp = DateTime.Now,
                Data = data,
                IsCompleted = false,
                Duration = TimeSpan.Zero,
                Status = "Pending"
            };

            this.Jobs.Add(job);

            return job;
        }
    }
}
