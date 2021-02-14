using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
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
    public class JobController : ControllerBase
    {
        private readonly ILogger<JobController> logger;
        private readonly IRepository repository;
        private readonly IBackgroundTaskQueue taskQueue;
        private readonly IMapper mapper;
        private readonly IWorker worker;

        public JobController(ILogger<JobController> logger,
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

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Job))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet]
        public ActionResult<IEnumerable<Job>> Get()
        {
            this.logger.LogInformation($"'{nameof(Get)}' has been invoked.");

            if (!this.repository.GetJobs().Any()) return NoContent();

            return this.repository.GetJobs().ToArray();
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Job))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public ActionResult<Job> Get(int id)
        {
            this.logger.LogInformation($"'{nameof(Get)}' has been invoked with id '{id}'.");
            
            if(id == -1)
            {
                throw new ArgumentException("This is just a demo for the error page.");
            }

            var job = this.repository.GetJob(id);

            if (job == null) return NotFound();

            return job;
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public ActionResult<int> Create([FromBody] JobRequest request)
        {
            this.logger.LogInformation($"'{nameof(Create)}' has been invoked.");

            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Save item to storage
            var job = this.mapper.Map<Job>(request);
            var id = this.repository.Upsert(job);
            job.Id = id;

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
            
            return CreatedAtAction(nameof(Get), new { id = id }, job);
        }

        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut]
        public ActionResult<int> Put([FromBody] Job job)
        {
            // Update existing job
            this.logger.LogInformation($"'{nameof(Put)}' has been invoked for id '{job.Id}'.");

            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            // Save item to storage
            var id = this.repository.Upsert(job);
            job.Id = id;

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
                    this.logger.LogError("Queued background task with id {Id} failed with message: {Message}", id, ex.Message);

                    job.Failed();
                }

                this.logger.LogInformation("Queued background task with id {Id} completed in {Duration} ticks", id,
                    job.Duration);

                // save data
                this.repository.Upsert(job);
            });
            
            return CreatedAtAction(nameof(Get), new { id = id }, job);
        }

        // TODO JsonPatch
        // https://docs.microsoft.com/en-us/aspnet/core/web-api/jsonpatch?view=aspnetcore-5.0

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // Delete job
            this.logger.LogInformation($"'{nameof(Delete)}' has been invoked for id '{id}'.");

            // get work item from storage
            var job = this.repository.GetJob(id);

            if (job == null) return NotFound();
            
            this.repository.DeleteJob(id);

            return Ok();
        }
    }
}