namespace Fanex.NexHR.Controllers
{
    using Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
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
        private readonly IOvertimeService _overtimeService;
        private readonly IEmployeeService _employeeService;

        public OvertimeController()
            : this(null, null)
        {
        }

        public OvertimeController(
            IOvertimeService service,
            IEmployeeService employeeService)
        {
            _overtimeService = service ?? new OvertimeService();
            _employeeService = employeeService ?? new EmployeeService();
        }

        public ActionResult Index(OvertimeSummaryFilterViewModel model)
        {
            if (model.EmployeeId != 0)
            {
                var criteria = new GetEmployeeCriteria { Id = model.EmployeeId };
                model.Employee = _employeeService.GetEmployeeById(criteria).DisplayName;
            }

            return View(model);
        }

        public ActionResult Create()
        {
            var model = new OvertimeCreateViewModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(OvertimeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.TimeSheets = JsonConvert.DeserializeObject<IList<OvertimeRecord>>(model.TimeSheetData);
                var criteria = new InsertOvertimeCriteria
                {
                    CreatedBy = CurrentUser.Id,
                    Description = model.Description,
                    Date = model.Date,
                    Status = model.IsApprove ? (int)RequestStatuse.Approve : (int)RequestStatuse.Pending,
                    TimeSheets = model.TimeSheets
                };

                var result = _overtimeService.Insert(criteria);
                if (result.Code == 0)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(result.Code.ToString(), result.Message);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Summary(OvertimeSummaryFilterViewModel model)
        {
            var criteria = new GetSummaryCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                CreatedBy = model.CreatedBy,
                Status = model.Status,
                EmployeeId = model.EmployeeId
            };

            var result = _overtimeService.GetSummaryReport(criteria);

            return View(result);
        }

        [HttpPost]
        public ActionResult ApproveRequest(int id)
        {
            var criteria = new ApproveOvertimeRequestCriteria
            {
                Id = id,
                Status = (int)RequestStatuse.Approve,
                UserId = CurrentUser.Id
            };

            var result = _overtimeService.ApproveRequest(criteria);

            return Json(result);
        }

        [HttpPost]
        public ActionResult RejectRequest(int id)
        {
            var criteria = new RejectRequestCriteria
            {
                Id = id,
                Status = (int)RequestStatuse.Reject,
                UserId = CurrentUser.Id
            };

            var result = _overtimeService.RejectRequest(criteria);

            return Json(result);
        }

        [HttpPost]
        public ActionResult DisapproveRequest(int id)
        {
            var criteria = new DisapproveOvertimeRequestCriteria
            {
                Id = id,
                Status = (int)RequestStatuse.Pending,
                UserId = CurrentUser.Id
            };

            var result = _overtimeService.DisapproveRequest(criteria);

            return Json(result);
        }

        public ActionResult Edit(int id)
        {
            var criteria = new GetByIdCriteria { Id = id };
            var model = _overtimeService.Get(criteria);

            return View(OvertimeEditViewModel.Parse(model));
        }

        [HttpPost]
        public ActionResult Edit(OvertimeEditViewModel model)
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
                    ModifiedBy = CurrentUser.Id
                };
                var result = _overtimeService.Update(criteria);
                if (result.Code == 0)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(result.Code.ToString(), result.Message);
            }

            return View(model);
        }
    }
}
