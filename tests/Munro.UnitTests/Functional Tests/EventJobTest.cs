using System.Collections.Generic;
using EventManager.WebAPI.Model;
using Xunit;

namespace Munro.UnitTests.Functional_Tests
{
    public class EventJobTest
    {
        public static IEnumerable<object[]> TestData
        {
            get
            {
                yield return new object[] {1, "Johnny", "JRN", "Pending", new[] {1, 2, 3}};
                yield return new object[] {2, "Keira", "JRN", "Pending", new[] {-1, -2, -3}};
                yield return new object[] {3, "Orlando", "JRN", "Pending", new[] {1000, 2000, 3000}};
                yield return new object[] {4, "Geoffrey", "JRN", "Pending", new[] {0, 0, 0}};
                yield return new object[] {5, "Kevin", "JRN", "Pending", new int[0]};
            }
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void can_create_eventjob(int id, string name, string user, string status, int[] data)
        {
            // Act
            var job = new EventJob
            {
                Id = id,
                Name = name,
                UserName = user,
                Data = data
            };

            // Assert
            Assert.Equal(id, job.Id);
            Assert.Equal(name, job.Name);
            Assert.Equal(user, job.UserName);
            Assert.Equal(status, job.Status);
            Assert.Equal(data, job.Data);
        }
    }
}