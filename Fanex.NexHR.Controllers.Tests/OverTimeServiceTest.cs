using Fanex.NexHR.Criteria.OverTime;
using Fanex.NexHR.Repositories.Interfaces;
using Fanex.NexHR.Services;
using Fanex.NexHR.Services.Interfaces;
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
        private OverTimeViewModel _model;
        private IOverTimeRepository _repository;
        private IOverTimeService _service;
        [SetUp]
        public void Setup()
        {
            _model = new OverTimeViewModel();
            _repository = Substitute.For<IOverTimeRepository>();
            _service = new OverTimeService(_repository);
        }


        [Test]
        public void Insert_ReturnTrue_WhenRepositoryReturnTrue()
        {
            // Arrange                        
            var criteria = new InsertOverTimeCriteria();

            // Action
            _repository.Insert(criteria).Returns(true);
            var result = _service.Insert(_model);

            // Assert
            Assert.IsTrue(result);
        }
    }
}