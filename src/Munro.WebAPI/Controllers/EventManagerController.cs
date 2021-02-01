using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EventManager.WebAPI.Model;
using EventManager.WebAPI.Services;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EventManager.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventManagerController : ControllerBase
    {
        private readonly JobContext db = new JobContext();
        private readonly ILogger<EventManagerController> logger;
        private readonly IMapper mapper;

        public EventManagerController(ILogger<EventManagerController> logger,
            IMapper mapper)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet]
        public ActionResult<IEnumerable<EventJob>> Get()
        {
            this.logger.LogInformation($"'{nameof(Get)}' has been invoked.");

            if (!this.db.Jobs.Any()) return NoContent();

            return this.db.Jobs.ToArray();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet("byId/{id}", Name = "GetById")]
        public ActionResult<EventJob> GetById(int id)
        {
            this.logger.LogInformation($"'{nameof(GetById)}' has been invoked with id '{id}'.");

            var job = this.db.Jobs.Find(id);

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

            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Save item to storage
            var job = this.mapper.Map<EventJob>(request);
            job.TimeStamp = DateTime.UtcNow;
            this.db.Jobs.Add(job);
            this.db.SaveChanges();

            var id = job.Id;

            BackgroundJob.Enqueue(() => DoWork(id));

            return id;
        }

        // Must be static or Hangfire will fail
        public static void DoWork(int id)
        {
            using var db = new JobContext();
            var job = db.Jobs.Find(id);
            if (job == null) return;
            job.Data = Worker.Sort(job.Data);
            job.Complete();
            db.Jobs.Update(job);
            db.SaveChanges();
        }
    }
}