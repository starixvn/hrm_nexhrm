using Fanex.NexHR.Services.Interfaces;
using Fanex.NexHR.ViewModels;
using NSubstitute;
using NUnit.Framework;
using System.Web.Mvc;

namespace Fanex.NexHR.Controllers.Tests
{
    [TestFixture]
    public class OverTimeControllerTest
    {
        private OverTimeController _controller;
        private OverTimeViewModel _model;
        private IOverTimeService _service;

        [SetUp]
        public void Setup()
        {
            _service = Substitute.For<IOverTimeService>();
            _controller = new OverTimeController(_service);
            _model = new OverTimeViewModel();            
        }

        [Test]
        public void Create_ReturnView()
        {
            // Arrange            

            // Action
            var result = _controller.Create() as ViewResult;

            // Assert
            Assert.AreEqual("Create", result.ViewName);
        }

        [Test]
        public void Create_ReturnModel()
        {
            // Arrange

            // Action
            var result = _controller.Create() as ViewResult;

            // Assert
            Assert.IsNotNull(result.Model);
        }

        [Test]
        public void Create_ForwardSummaryAction_AfterCreateSuccessful()
        {
            // Arrange
            _service.Insert(_model).Returns(true);

            // Action
            var result = _controller.Create(_model) as RedirectToRouteResult;

            // Assert
            _service.Received().Insert(_model);
            Assert.AreEqual("Summary", result.RouteValues["Action"]);
        }

    }
}