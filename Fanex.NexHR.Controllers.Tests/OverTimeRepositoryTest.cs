using Fanex.Data.Configuration;
using Fanex.NexHR.Models;
using Fanex.NexHR.Repositories;
using Fanex.NexHR.ViewModels;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fanex.NexHR.Controllers.Tests
{
    [TestFixture]
    public class OverTimeRepositoryTest
    {
        [SetUp]
        public void Setup()
        {
            DbSettingProviderManager.Current.Start();
        }

        [Test]
        public void Insert_Pass()
        {
            // Arrange
            var repository = new OverTimeRepository();
            var model = new OverTimeViewModel()
            {
                CreatedId = 263,
                Description = "NexHR",
                Date = DateTime.Now,
                Records = new List<OverTimeRecord>() { new OverTimeRecord() { EmployeeId = 263, Hour = 3 }, new OverTimeRecord() { EmployeeId = 265, Hour = 3 } }
            };

            // Action
            var result = repository.Insert(model);

            // Assert
            Assert.Pass();
        }
    }
}