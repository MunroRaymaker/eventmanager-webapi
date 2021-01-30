using EventManager.WebAPI.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventManager.WebAPI.Model;

namespace EventManager.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventManagerController : ControllerBase
    {
        private readonly ILogger<EventManagerController> logger;

        private readonly List<EventJob> jobs;
        
        public EventManagerController(ILogger<EventManagerController> logger)
        {
            this.logger = logger;

            this.jobs = new List<EventJob>();
            
            // Add some fake data
            for (int i = 0; i < 100; i++)
            {
                this.jobs.Add(new EventJob
                {
                    Id = i,
                    IsCompleted = false,
                    Status = "Pending",
                    TimeStamp = DateTime.UtcNow,
                    Duration = TimeSpan.Zero,
                    Data = Enumerable.Range(1,10).Shuffle().ToArray()
                });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet]
        public ActionResult<IEnumerable<EventJob>> Get()
        {
            this.logger.LogInformation($"'{nameof(Get)}' has been invoked.");

            if (!this.jobs.Any())
            {
                return NoContent();
            }

            return this.jobs;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet("byId/{id}", Name = "GetById")]
        public async Task<ActionResult<EventJob>> GetById(int id)
        {
            this.logger.LogInformation($"'{nameof(GetById)}' has been invoked with id '{id}'.");

            var job = this.jobs.FirstOrDefault(x => x.Id == id);

            if (job == null) return NoContent();

            return job;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("AddJob", Name = "AddJob")]
        public ActionResult<EventJob> AddJob([FromBody] EventJobRequest request)
        {
            this.logger.LogInformation($"'{nameof(AddJob)}' has been invoked.");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var job = new EventJob
            {
                Id = this.jobs.Any() ? this.jobs.Max(x => x.Id) + 1 : 1,
                TimeStamp = DateTime.Now,
                Data = request.Data,
                Name = request.Name,
                UserName = request.UserName,
                IsCompleted = false,
                Duration = TimeSpan.Zero,
                Status = "Pending"
            };

            this.jobs.Add(job);

            return job;
        }
    }
}
