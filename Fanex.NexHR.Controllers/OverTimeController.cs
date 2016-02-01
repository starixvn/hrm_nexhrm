namespace Fanex.NexHR.Controllers
{
    using Core.Enums;
    using Criteria.Employee;
    using Fanex.NexHR.Criteria.OverTime;
    using Fanex.NexHR.Models;
    using Newtonsoft.Json;
    using Services;
    using Services.Interfaces;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using ViewModels;

    public class OverTimeController : BaseController
    {
        private readonly IOverTimeService _service;
        private readonly IEmployeeService _employeeService;

        public OverTimeController() : this(null, null) { }

        public OverTimeController(IOverTimeService service,
            IEmployeeService employeeService = null)
        {
            _service = service ?? new OverTimeService();
            _employeeService = employeeService ?? new EmployeeService();
        }

        [Authorize(Roles = "AssistantLeader,Leader")]
        public ActionResult Create()
        {
            var model = new OverTimeViewModel();

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "AssistantLeader,Leader")]
        public ActionResult Create(OverTimeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.RecordList = JsonConvert.DeserializeObject<IList<OverTimeRecord>>(model.RecordData);
            var criteria = new InsertOverTimeCriteria
            {
                CreatedBy = CurrentUser.Id,
                Description = model.Description,
                Date = model.Date,
                Status = model.IsApprove ? (int)RequestStatuse.Approve : (int)RequestStatuse.Pending,
                OverTimeRecord = model.RecordList
            };

            var result = _service.Insert(criteria);
            if (result.Code != 0)
            {
                ModelState.AddModelError(result.Code.ToString(), result.Message);
                return View(model);
            }

            return RedirectToAction("Summary");
        }

        [HttpPost]
        public ActionResult SummaryResult(OverTimeSummaryFilterViewModel model)
        {
            var criteria = new GetSummaryCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                CreatedBy = model.CreatedBy,
                Status = model.Status,
                EmployeeId = model.EmployeeId
            };

            var result = _service.GetSummaryReport(criteria);

            return View("SummaryResult", result);
        }

        public ActionResult Summary(OverTimeSummaryFilterViewModel model)
        {
            if (model.EmployeeId != 0)
            {
                var criteria = new GetEmployeeCriteria { Id = model.EmployeeId };
                model.Employee = _employeeService.GetEmployeeById(criteria).DisplayName;
            }

            return View("Summary", model);
        }

        [HttpPost]
        [Authorize(Roles = "AssistantLeader,Leader")]
        public ActionResult ApproveRequest(int id)
        {
            var criteria = new ApproveOverTimeRequestCriteria
            {
                Id = id,
                Status = (int)RequestStatuse.Approve,
                UserId = CurrentUser.Id
            };

            var result = _service.ApproveRequest(criteria);

            return Json(result);
        }

        [HttpPost]
        [Authorize(Roles = "AssistantLeader,Leader")]
        public ActionResult RejectRequest(int id)
        {
            var criteria = new UpdateStatusCriteria
            {
                Id = id,
                Status = (int)RequestStatuse.Reject,
                UserId = CurrentUser.Id
            };

            var result = _service.RejectRequest(criteria);

            return Json(result);
        }

        [HttpPost]
        [Authorize(Roles = "AssistantLeader,Leader")]
        public ActionResult DisapproveRequest(int id)
        {
            var criteria = new DisapproveOverTimeRequestCriteria
            {
                Id = id,
                Status = (int)RequestStatuse.Pending,
                UserId = CurrentUser.Id
            };

            var result = _service.DisapproveRequest(criteria);

            return Json(result);
        }

        [Authorize(Roles = "AssistantLeader,Leader")]
        public ActionResult Edit(int id)
        {
            var criteria = new GetByIdCriteria { Id = id };
            var model = _service.Get(criteria);

            return View(OverTimeViewModel.Parse(model));
        }

        [HttpPost]
        [Authorize(Roles = "AssistantLeader,Leader")]
        public ActionResult Edit(OverTimeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var criteria = new UpdateOverTimeCriteria
                {
                    Id = model.Id,
                    Description = model.Description,
                    Date = model.Date,
                    Hour = model.Hour,
                    EmployeeId = model.EmployeeId,
                    ModifyBy = CurrentUser.Id
                };
                var result = _service.Update(criteria);
                if (result.Code == 0)
                {
                    return RedirectToAction("Summary");
                }

                ModelState.AddModelError(result.Code.ToString(), result.Message);
            }

            return View(model);
        }
    }
}
