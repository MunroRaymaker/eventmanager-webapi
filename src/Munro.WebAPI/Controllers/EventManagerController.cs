using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EventManager.WebAPI.Model;
using EventManager.WebAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EventManager.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventManagerController : ControllerBase
    {
        private readonly ILogger<EventManagerController> logger;
        private readonly IRepository repository;
        private readonly IBackgroundTaskQueue taskQueue;
        private readonly IMapper mapper;
        private readonly IWorker worker;

        public EventManagerController(ILogger<EventManagerController> logger,
            IBackgroundTaskQueue taskQueue,
            IRepository repository, 
            IMapper mapper,
            IWorker worker)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.taskQueue = taskQueue ?? throw new ArgumentNullException(nameof(taskQueue));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.worker = worker ?? throw new ArgumentNullException(nameof(worker));
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet]
        public ActionResult<IEnumerable<EventJob>> Get()
        {
            this.logger.LogInformation($"'{nameof(Get)}' has been invoked.");

            if (!this.repository.GetJobs().Any()) return NoContent();

            return this.repository.GetJobs().ToArray();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public ActionResult<EventJob> Get(int id)
        {
            this.logger.LogInformation($"'{nameof(Get)}' has been invoked with id '{id}'.");

            var job = this.repository.GetJob(id);

            if (job == null) return NotFound();

            return job;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public ActionResult<int> Create([FromBody] EventJobRequest request)
        {
            this.logger.LogInformation($"'{nameof(Create)}' has been invoked.");

            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Save item to storage
            var id = this.repository.Upsert(this.mapper.Map<EventJob>(request));

            this.taskQueue.QueueBackgroundWorkItem(async token =>
            {
                this.logger.LogInformation("Queued background task with id {Id} started ", id);

                // get work item from storage
                var job = this.repository.GetJob(id);
                
                try
                {
                    // sort data
                    job.Data = worker.DoWork(job.Data);

                    // Add some delay
                    await Task.Delay(TimeSpan.FromSeconds(2), token);

                    job.Complete();
                }
                catch (Exception ex)
                {
                    // What if DoWork fails? Mark as failed and refer to manual processing.
                    this.logger.LogError("Queued background task with id {Id} failed with message {Message}", id, ex.Message);

                    job.Failed();
                }

                this.logger.LogInformation("Queued background task with id {Id} completed in {Duration} ticks", id,
                    job.Duration);

                // save data
                this.repository.Upsert(job);

            });

            return id;
        }
    }
}