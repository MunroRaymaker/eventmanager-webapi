using EventManager.WebAPI.Model;
using FluentAssertions;
using System;
using Xunit;

namespace Munro.UnitTests.Functional_Tests
{
    public class MappingTests : BaseTest
    {
        [Fact]
        public void should_have_valid_configuration()
        {
            this.Configuration.AssertConfigurationIsValid();
        }

        [Theory]
        [InlineData(typeof(JobRequest), typeof(Job))]
        public void should_support_mapping_from_source_to_destination(Type source, Type destination)
        {
            var instance = Activator.CreateInstance(source);

            this.Mapper.Map(instance, source, destination);
        }

        [Fact]
        public void when_eventrequest_should_map_to_eventjob()
        {
            // Arrange
            var request = new JobRequest
            {
                Data = new []{ 5, 2, 48 },
                Name = "Foo",
                UserName = "Bar"
            };

            // Act
            var job = this.Mapper.Map<Job>(request);

            // Assert
            job.UserName.Should().Be("Bar");
            job.Name.Should().Be("Foo");
            job.Data.Should().ContainInOrder(new[] {5, 2, 48});
        }
    }
}
