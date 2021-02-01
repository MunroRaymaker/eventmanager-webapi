using EventManager.WebAPI.Services;
using FluentAssertions;
using Xunit;

namespace Munro.UnitTests.Functional_Tests
{
    public class ArraySorterTest
    {
        [Theory]
        [InlineData(new[] { 8, 9, 3 })]
        [InlineData(new[] { 3000, 2000, 1000 })]
        [InlineData(new[] { 0, -9, -3 })]
        public void when_sorting_array_should_be_in_sorted_order(int[] input)
        {
            // Act
            var actual = Worker.Sort(input);

            // Assert
            actual.Should().BeInAscendingOrder("Because it was sorted");
        }
    }
}
