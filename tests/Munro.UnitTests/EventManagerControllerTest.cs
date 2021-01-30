using EventManager.WebAPI.Controllers;
using EventManager.WebAPI.Model;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq;
using Xunit;

namespace Munro.UnitTests
{
    public class EventManagerControllerTest
    {
        [Fact]
        public void when_getting_all_jobs_should_return_jobs()
        {
            var logger = Mock.Of<ILogger<EventManagerController>>();

            // Arrange
            var controller = new EventManagerController(logger);

            // Act
            var response = controller.Get();
            
            // Assert
            EventJob[] jobs = response.Value.ToArray();
            jobs.Should().BeOfType<EventJob[]>();
            
            //.StatusCode.Should().Be(StatusCodes.Status200OK);
        }
    }
}
