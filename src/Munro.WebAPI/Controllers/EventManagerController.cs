using EventManager.WebAPI.Model;
using EventManager.WebAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventManager.WebAPI.Mapping;

namespace EventManager.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventManagerController : ControllerBase
    {
        private readonly ILogger<EventManagerController> logger;
        private readonly IBackgroundTaskQueue taskQueue;
        private readonly IRepository repository;

        public EventManagerController(ILogger<EventManagerController> logger,
            IBackgroundTaskQueue taskQueue,
            IRepository repository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.taskQueue = taskQueue ?? throw new ArgumentNullException(nameof(taskQueue));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet]
        public ActionResult<IEnumerable<EventJob>> Get()
        {
            this.logger.LogInformation($"'{nameof(Get)}' has been invoked.");

            if (!this.repository.GetJobs().Any())
            {
                return NoContent();
            }

            return this.repository.GetJobs().ToArray();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet("byId/{id}", Name = "GetById")]
        public async Task<ActionResult<EventJob>> GetById(int id)
        {
            this.logger.LogInformation($"'{nameof(GetById)}' has been invoked with id '{id}'.");

            var job = this.repository.GetJob(id);

            if (job == null) return NoContent();

            return job;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("AddJob", Name = "AddJob")]
        public ActionResult<int> AddJob([FromBody] EventJobRequest request)
        {
            this.logger.LogInformation($"'{nameof(AddJob)}' has been invoked.");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Save item to storage
            int id = this.repository.Upsert(Mapper.Map(request));
            
            this.taskQueue.QueueBackgroundWorkItem(async token => {

                this.logger.LogInformation("Queued background task with id {Id} started ", id);
                
                // get work item from storage
                var job = this.repository.GetJob(id);

                // sort data
                job.Data = LongTasks.Sort(job.Data);
                job.Complete();

                this.logger.LogInformation("Queued background task with id {Id} completed in {Duration} ticks", id, job.Duration);

                // save data
                this.repository.Upsert(job);
            });
            
            return id;
        }
    }
}
