using Fanex.NexHR.Repositories.Interfaces;
using Fanex.NexHR.Services;
using Fanex.NexHR.ViewModels;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fanex.NexHR.Controllers.Tests
{
    [TestFixture]
    public class OverTimeServiceTest
    {
        [Test]
        public void Create_CallRepository()
        {
            // Arrange            
            var repository = Substitute.For<IOverTimeRepository>();
            var service = new OverTimeService(repository);
            var model = new OverTimeViewModel();

            // Action
            var result = service.Insert(model);

            // Assert
            repository.Received().Insert(model);
        }

        [Test]
        public void Create_ReturnTrue_WhenRepositoryReturnTrue()
        {
            // Arrange            
            var repository = Substitute.For<IOverTimeRepository>();
            var service = new OverTimeService(repository);
            var model = new OverTimeViewModel();

            // Action
            repository.Insert(model).Returns(true);
            var result = service.Insert(model);

            // Assert
            Assert.IsTrue(result);
        }
    }
}