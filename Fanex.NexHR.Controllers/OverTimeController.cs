using Fanex.NexHR.Services.Interfaces;
using Fanex.NexHR.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Fanex.NexHR.Services;
using Fanex.NexHR.Models;
using Newtonsoft.Json;
using Fanex.NexHR.Core.Enums;

namespace Fanex.NexHR.Controllers
{
    public class OverTimeController : BaseController
    {
        private readonly IOverTimeService _service;

        public OverTimeController() : this(null) { }

        public OverTimeController(IOverTimeService service)
        {
            _service = service ?? new OverTimeService();
        }

        public ActionResult Create()
        {
            var model = new OverTimeViewModel();

            return View("Create", model);
        }

        [HttpPost]
        public ActionResult Create(OverTimeViewModel model)
        {
            var list = JsonConvert.DeserializeObject<List<OverTimeRecord>>(model.RecordData);
            model.Records = list;
            model.CreatedBy = this.CurrentUser.Id;
            model.Status = (int)OTRequestStatus.New;

            _service.Insert(model);

            return RedirectToAction("Summary");
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
        public ActionResult ApproveRequest(int id)
        {
            var result = _service.UpdateStatus(id, OTRequestStatus.Approved);

            return Json(result);
        }

        [HttpPost]
        public ActionResult VerifyRequest(int id)
        {
            var result = _service.UpdateStatus(id, OTRequestStatus.Verified);

            return Json(result);
        }
        
        public ActionResult Edit(int id)
        {
            var model = _service.Get(id);

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(OverTime model)
        {
            var result = _service.Update(model);

            var viewModel = new OverTimeSummaryFilterViewModel();

            return View("Summary", viewModel);
        }
    }
}
