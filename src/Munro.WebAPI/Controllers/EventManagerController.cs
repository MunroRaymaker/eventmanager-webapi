using EventManager.WebAPI.Model;
using EventManager.WebAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventManager.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventManagerController : ControllerBase
    {
        private readonly ILogger<EventManagerController> logger;
        private readonly IProcessingService processingService;
        
        public EventManagerController(ILogger<EventManagerController> logger, IProcessingService processingService)
        {
            this.logger = logger;
            this.processingService = processingService;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet]
        public ActionResult<IEnumerable<EventJob>> Get()
        {
            this.logger.LogInformation($"'{nameof(Get)}' has been invoked.");

            if (!ProcessingService.Jobs.Any())
            {
                return NoContent();
            }

            return ProcessingService.Jobs;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet("byId/{id}", Name = "GetById")]
        public async Task<ActionResult<EventJob>> GetById(int id)
        {
            this.logger.LogInformation($"'{nameof(GetById)}' has been invoked with id '{id}'.");

            var job = ProcessingService.Jobs.FirstOrDefault(x => x.Id == id);

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
                Id = ProcessingService.Jobs.Any() ? ProcessingService.Jobs.Max(x => x.Id) + 1 : 1,
                TimeStamp = DateTime.Now,
                Data = request.Data,
                Name = request.Name,
                UserName = request.UserName,
                IsCompleted = false,
                Duration = 0,
                Status = "Pending"
            };

            ProcessingService.Jobs.Enqueue(job);

            return job;
        }
    }
}
