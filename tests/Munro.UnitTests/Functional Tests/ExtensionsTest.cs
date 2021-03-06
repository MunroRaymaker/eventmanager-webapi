﻿using EventManager.WebAPI.Extensions;
using FluentAssertions;
using Xunit;

namespace Munro.UnitTests.Functional_Tests
{
    public class ExtensionsTest
    {
        [Theory]
        [InlineData(new []{ 1,2,3,4,5,6,7 })]
        public void when_shuffling_array_should_be_in_unsorted_order(int[] input)
        {
            // Act
            var actual = input.Shuffle();

            // Assert
            actual.Should().NotBeInAscendingOrder("Because it was shuffled");
            actual.Should().NotBeInDescendingOrder("Because it was shuffled");
        }
    }
}
