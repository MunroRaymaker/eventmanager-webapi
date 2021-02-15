using System;
using System.Collections.Generic;
using EventManager.WebAPI.Controllers;
using EventManager.WebAPI.Model;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using EventManager.WebAPI.Services;
using Xunit;

namespace Munro.UnitTests
{
    public class JobsControllerTest : BaseTest
    {
        [Fact]
        public async Task when_getting_all_jobs_should_return_jobs()
        {
            // Arrange
            var logger = Mock.Of<ILogger<JobsController>>();
            var btq = Mock.Of<IBackgroundTaskQueue>();
            var repo = new Mock<IRepository>();
            var db = Mock.Of<JobContext>();
            var worker = new Worker();
            repo.Setup(x => x.GetJobs()).Returns(TestData);
            var controller = new JobsController(logger, btq, this.Mapper, worker, db);

            // Act
            var response = await controller.Get();

            // Assert
            Job[] jobs = response.Value.ToArray();
            jobs.Should().BeOfType<Job[]>();
            jobs.Length.Should().BeGreaterThan(0);

            //.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        private static IEnumerable<Job> TestData
        {
            get
            {
                yield return new Job
                {
                    Id = 1, Name = "Johnny", UserName = "JRN", TimeStamp = DateTime.Now, 
                    Data = new[] {3, 2, 1}
                };
                yield return new Job
                {
                    Id = 2, Name = "Keira", UserName = "JRN", TimeStamp = DateTime.Now, 
                    Data = new[] {5, 4, 3}
                };
                yield return new Job
                {
                    Id = 3, Name = "Orlando", UserName = "JRN", TimeStamp = DateTime.Now, 
                    Data = new[] {3000, 5000, 2000, 8000}
                };
            }
        }
    }
}
