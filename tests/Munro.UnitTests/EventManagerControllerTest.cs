using System;
using System.Collections.Generic;
using EventManager.WebAPI.Model;

namespace Munro.UnitTests
{
    public class EventManagerControllerTest : BaseTest
    {
        //[Fact]
        //public void when_getting_all_jobs_should_return_jobs()
        //{
        //    // Arrange
        //    var logger = Mock.Of<ILogger<EventManagerController>>();
        //    var controller = new EventManagerController(logger, this.Mapper);

        //    // Act
        //    var response = controller.Get();

        //    // Assert
        //    EventJob[] jobs = response.Value.ToArray();
        //    jobs.Should().BeOfType<EventJob[]>();
        //    jobs.Length.Should().BeGreaterThan(0);

        //    //.StatusCode.Should().Be(StatusCodes.Status200OK);
        //}

        private static IEnumerable<EventJob> TestData
        {
            get
            {
                yield return new EventJob
                {
                    Id = 1, Name = "Johnny", UserName = "JRN", TimeStamp = DateTime.Now,
                    Data = new[] {3, 2, 1}
                };
                yield return new EventJob
                {
                    Id = 2, Name = "Keira", UserName = "JRN", TimeStamp = DateTime.Now,
                    Data = new[] {5, 4, 3}
                };
                yield return new EventJob
                {
                    Id = 3, Name = "Orlando", UserName = "JRN", TimeStamp = DateTime.Now,
                    Data = new[] {3000, 5000, 2000, 8000}
                };
            }
        }
    }
}