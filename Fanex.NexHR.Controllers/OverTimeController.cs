using Fanex.NexHR.Core.Enums;
using Fanex.NexHR.Services;
using Fanex.NexHR.Services.Interfaces;
using Fanex.NexHR.ViewModels;
using System.Web.Mvc;
using System.Web.Security;

namespace Fanex.NexHR.Controllers
{
    [Authorize]
    public class OverTimeController : BaseController
    {
        private readonly IOverTimeService _service;

        public OverTimeController() : this(null) { }

        public OverTimeController(IOverTimeService service)
        {
            _service = service ?? new OverTimeService();
        }

        [Authorize(Roles = "AssistantLeader,Leader")]
        public ActionResult Create()
        {
            var model = new OverTimeViewModel();

            return View("Create", model);
        }

        [HttpPost]
        [Authorize(Roles = "AssistantLeader,Leader")]
        public ActionResult Create(OverTimeViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedId = this.CurrentUser.Id;
                model.Status = model.IsApprove ? (int)OTRequestStatus.Approved : (int)OTRequestStatus.Pending;
                _service.Insert(model);

                return RedirectToAction("Summary");
            }

            return View();
        }

        public ActionResult Summary()
        {
            var model = new OverTimeSummaryFilterViewModel();

            return View("Summary", model);
        }

        [HttpPost]
        public ActionResult Summary(OverTimeSummaryFilterViewModel model)
        {
            var result = _service.GetSummaryReport(model);

            return View("SummaryResult", result);
        }

        [HttpPost]
        [Authorize(Roles = "AssistantLeader,Leader")]
        public ActionResult ApproveRequest(int id)
        {
            var result = _service.ApproveRequest(id, this.CurrentUser.Id);

            return Json(result);
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult VerifyRequest(int id)
        {
            var result = _service.VerifyRequest(id, this.CurrentUser.Id);

            return Json(result);
        }

        [Authorize(Roles = "AssistantLeader,Leader")]
        public ActionResult Edit(int id)
        {
            var model = _service.Get(id);

            return View(OverTimeViewModel.Parse(model));
        }

        [HttpPost]
        [Authorize(Roles = "AssistantLeader,Leader")]
        public ActionResult Edit(OverTimeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _service.Update(model);

                return RedirectToAction("Summary");
            }

            return View();
        }
    }
}
