namespace Fanex.NexHR.Controllers
{
    using Core.Enums;
    using Criteria.Employee;
    using Fanex.NexHR.Criteria.Overtime;
    using Fanex.NexHR.Models;
    using Newtonsoft.Json;
    using Services;
    using Services.Interfaces;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using ViewModels;

    public class OvertimeController : BaseController
    {
        private readonly IOvertimeService _service;
        private readonly IEmployeeService _employeeService;

        public OvertimeController() : this(null, null) { }

        public OvertimeController(IOvertimeService service,
            IEmployeeService employeeService = null)
        {
            _service = service ?? new OvertimeService();
            _employeeService = employeeService ?? new EmployeeService();
        }

        [Authorize(Roles = "AssistantLeader,Leader")]
        public ActionResult Create()
        {
            var model = new OvertimeViewModel();

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "AssistantLeader,Leader")]
        public ActionResult Create(OvertimeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.RecordList = JsonConvert.DeserializeObject<IList<OvertimeRecord>>(model.RecordData);
            var criteria = new InsertOvertimeCriteria
            {
                CreatedBy = CurrentUser.Id,
                Description = model.Description,
                Date = model.Date,
                Status = model.IsApprove ? (int)RequestStatuse.Approve : (int)RequestStatuse.Pending,
                OvertimeRecord = model.RecordList
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
        public ActionResult SummaryResult(OvertimeSummaryFilterViewModel model)
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

        public ActionResult Summary(OvertimeSummaryFilterViewModel model)
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
            var criteria = new ApproveOvertimeRequestCriteria
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
            var criteria = new DisapproveOvertimeRequestCriteria
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

            return View(OvertimeViewModel.Parse(model));
        }

        [HttpPost]
        [Authorize(Roles = "AssistantLeader,Leader")]
        public ActionResult Edit(OvertimeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var criteria = new UpdateOvertimeCriteria
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
