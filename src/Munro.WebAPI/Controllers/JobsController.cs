using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EventManager.WebAPI.Model;
using EventManager.WebAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventManager.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly ILogger<JobsController> logger;
        private readonly IMapper mapper;
        private readonly JobContext db;
        private readonly IBackgroundTaskQueue taskQueue;
        private readonly IWorker worker;

        public JobsController(ILogger<JobsController> logger,
            IBackgroundTaskQueue taskQueue,
            IMapper mapper,
            IWorker worker, 
            JobContext db)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.taskQueue = taskQueue ?? throw new ArgumentNullException(nameof(taskQueue));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.worker = worker ?? throw new ArgumentNullException(nameof(worker));
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        // GET: api/Jobs
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Job))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Job>>> Get()
        {
            this.logger.LogInformation($"'{nameof(Get)}' has been invoked.");

            var jobs = await this.db.JobItems.ToListAsync();

            if (!jobs.Any()) return NoContent();

            return jobs;
        }

        // GET: api/Jobs/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [HttpGet("{id}")]
        public async Task<ActionResult<Job>> Get(int id)
        {
            this.logger.LogInformation($"'{nameof(Get)}' has been invoked with id '{id}'.");

            if (id == -1) throw new ArgumentException("This is just a demo for the error page.");

            var job = await this.db.JobItems.FindAsync(id);

            if (job == null) return NotFound();

            return job;
        }

        // POST: api/Jobs
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<ActionResult<Job>> Create([FromBody] JobRequest request)
        {
            this.logger.LogInformation($"'{nameof(Create)}' has been invoked.");

            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Save item to storage
            var job = this.mapper.Map<Job>(request);
            this.db.JobItems.Add(job);
            await this.db.SaveChangesAsync();
            
            this.taskQueue.QueueBackgroundWorkItem(async token =>
            {
                await this.worker.DoWork(job.Id);
            });

            return CreatedAtAction(nameof(Get), new {job.Id}, job);
        }

        // PUT: api/Jobs/5
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [HttpPut]
        public async Task<ActionResult<int>> Put(long id, [FromBody] Job job)
        {
            // Update existing job
            this.logger.LogInformation($"'{nameof(Put)}' has been invoked for id '{job.Id}'.");

            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            if (id != job.Id) return BadRequest();

            this.db.Entry(job).State = EntityState.Modified;

            try
            {
                await this.db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!JobExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        // TODO JsonPatch
        // https://docs.microsoft.com/en-us/aspnet/core/web-api/jsonpatch?view=aspnetcore-5.0

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // Delete job
            this.logger.LogInformation($"'{nameof(Delete)}' has been invoked for id '{id}'.");
            
            var job = await this.db.JobItems.FindAsync(id);
            if (job == null) return NotFound();

            this.db.JobItems.Remove(job);
            await this.db.SaveChangesAsync();

            return Ok();
        }

        private bool JobExists(long id)
        {
            return this.db.JobItems.Any(e => e.Id == id);
        }
    }
}