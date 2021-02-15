using System;
using System.Linq;
using System.Threading.Tasks;
using EventManager.WebAPI.Controllers;
using EventManager.WebAPI.Model;
using EventManager.WebAPI.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Munro.UnitTests
{
    public class JobsControllerTest : ControllerTest
    {
        public JobsControllerTest()
            : base(new DbContextOptionsBuilder<JobContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options)
        {
        }

        [Fact]
        public async Task when_getting_all_jobs_should_return_jobs()
        {
            // Arrange
            var logger = Mock.Of<ILogger<JobsController>>();
            var btq = Mock.Of<IBackgroundTaskQueue>();
            var worker = new Worker();

            // Use a clean context to run tests
            await using (var context = new JobContext(ContextOptions))
            {
                var controller = new JobsController(logger, btq, this.Mapper, worker, context);

                // Act
                var response = await controller.Get();

                // Assert
                var jobs = response.Value.ToArray();
                jobs.Should().BeOfType<Job[]>();
                jobs.Length.Should().BeGreaterThan(0);
            }
        }

        [Fact]
        public async Task when_posting_returns_badrequest_when_model_is_invalid()
        {
            // Arrange
            var logger = Mock.Of<ILogger<JobsController>>();
            var btq = Mock.Of<IBackgroundTaskQueue>();
            var worker = new Worker();

            // Use a clean context to run tests
            await using (var context = new JobContext(ContextOptions))
            {
                var controller = new JobsController(logger, btq, this.Mapper, worker, context);

                controller.ModelState.AddModelError("Name", "Name cannot be empty");

                // Act
                var response = await controller.Create(new JobRequest());

                // Assert
                response.Result.Should().BeOfType<BadRequestObjectResult>();
            }
        }

        [Fact]
        public async Task when_posting_returns_location_when_model_is_valid()
        {
            // Arrange
            var logger = Mock.Of<ILogger<JobsController>>();
            var btq = Mock.Of<IBackgroundTaskQueue>();
            var worker = new Worker();

            // Use a clean context to run tests
            await using (var transaction = new JobContext(ContextOptions).Database.CurrentTransaction)
            await using (var context = new JobContext(ContextOptions))
            {
                var controller = new JobsController(logger, btq, this.Mapper, worker, context);

                // Act
                var response = await controller.Create(new JobRequest
                {
                    Data = new[] {3, 2, 1},
                    Name = "foo",
                    UserName = "bar"
                });

                // Assert
                response.Result.Should().BeOfType<CreatedAtActionResult>();
                var result = response.Result as CreatedAtActionResult;
                result.StatusCode.Should().Be(StatusCodes.Status201Created);

                await transaction.RollbackAsync();
            }
        }
    }
}