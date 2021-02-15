using System;
using System.Collections.Generic;
using EventManager.WebAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace Munro.UnitTests
{
    public class ControllerTest : BaseTest
    {
        public DbContextOptions<JobContext> ContextOptions { get; }

        public ControllerTest(DbContextOptions<JobContext> options)
        {
            ContextOptions = options;
            Seed();
        }

        private void Seed()
        {
            using (var ctx = new JobContext(ContextOptions))
            {
                ctx.JobItems.AddRange(TestData);
                ctx.SaveChanges();
            }
        }
        
        private static IEnumerable<Job> TestData
        {
            get
            {
                yield return new Job
                {
                    Name = "Johnny",
                    UserName = "JRN",
                    TimeStamp = DateTime.Now,
                    Data = new[] { 3, 2, 1 }
                };
                yield return new Job
                {
                    Name = "Keira",
                    UserName = "JRN",
                    TimeStamp = DateTime.Now,
                    Data = new[] { 5, 4, 3 }
                };
                yield return new Job
                {
                    Name = "Orlando",
                    UserName = "JRN",
                    TimeStamp = DateTime.Now,
                    Data = new[] { 3000, 5000, 2000, 8000 }
                };
            }
        }
    }
}