using EventManager.WebAPI.Model;
using EventManager.WebAPI.Validators;
using FluentAssertions;
using Xunit;

namespace Munro.UnitTests.Functional_Tests
{
    public class JobRequestValidationTest
    {
        [Fact]
        public void when_valid_jobrequest_should_pass_validation()
        {
            // Arrange
            var job = new JobRequest
            {
                Name = "Q1",
                UserName = "FOO",
                Data = new []{ 1, 2 }
            };

            var validator = new JobRequestValidator();
            
            // Act
            var result = validator.Validate(job);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void jobname_should_not_be_empty()
        {
            // Arrange
            var job = new JobRequest
            {
                Name = "",
                UserName = "FOO",
                Data = new []{ 1, 2 }
            };

            var validator = new JobRequestValidator();

            // Act
            var result = validator.Validate(job);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "Name");
        }

        [Fact]
        public void data_should_not_be_empty()
        {
            // Arrange
            var job = new JobRequest
            {
                Name = "Q1",
                UserName = "FOO",
                Data = new int[0]
            };

            var validator = new JobRequestValidator();

            // Act
            var result = validator.Validate(job);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "Data");
        }

        [Fact]
        public void username_should_not_be_empty()
        {
            // Arrange
            var job = new JobRequest
            {
                Name = "Q1",
                UserName = null,
                Data = new[] { 1, 2 }
            };

            var validator = new JobRequestValidator();

            // Act
            var result = validator.Validate(job);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == "UserName");
        }
    }
}
