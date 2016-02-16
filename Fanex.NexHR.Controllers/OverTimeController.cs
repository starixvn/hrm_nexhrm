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

        public ActionResult Index()
        {
            var model = new OvertimeFilterViewModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult ReportSummary(OvertimeFilterViewModel model)
        {
            var criteria = new ReportSummaryCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                CreatedBy = model.CreatedBy,
                Status = model.Status
            };

            IEnumerable<Overtime> result = _overtimeService.ReportSummary(criteria);

            return View(result);
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

                CommandResult result = _overtimeService.Insert(criteria);
                if (result.Code == 0)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(result.Code.ToString(), result.Message);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult ApproveRequest(int id)
        {
            var criteria = new ApproveOvertimeRequestCriteria
            {
                Id = id,
                Status = (int)RequestStatuse.Approve,
                ModifiedBy = CurrentUser.Id
            };

            CommandResult result = _overtimeService.ApproveRequest(criteria);

            return Json(result);
        }

        [HttpPost]
        public ActionResult RejectRequest(int id)
        {
            var criteria = new RejectRequestCriteria
            {
                Id = id,
                Status = (int)RequestStatuse.Reject,
                ModifiedBy = CurrentUser.Id
            };

            CommandResult result = _overtimeService.RejectRequest(criteria);

            return Json(result);
        }

        [HttpPost]
        public ActionResult DisapproveRequest(int id)
        {
            var criteria = new DisapproveOvertimeRequestCriteria
            {
                Id = id,
                Status = (int)RequestStatuse.Pending,
                ModifiedBy = CurrentUser.Id
            };

            CommandResult result = _overtimeService.DisapproveRequest(criteria);

            return Json(result);
        }

        public ActionResult Edit(int id)
        {
            var criteria = new GetByIdCriteria { Id = id };
            OvertimeCreateViewModel model = _overtimeService.Get(criteria);
            model.TimeSheetData = JsonConvert.SerializeObject(model.TimeSheets);

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(OvertimeCreateViewModel model)
        {
            if (ModelState.IsValid && HasPermission())
            {
                model.TimeSheets = JsonConvert.DeserializeObject<IList<OvertimeRecord>>(model.TimeSheetData);
                var criteria = new UpdateOvertimeCriteria
                {
                    Id = model.Id,
                    ModifiedBy = CurrentUser.Id,
                    Description = model.Description,
                    Date = model.Date,
                    Status = model.IsApprove ? (int)RequestStatuse.Approve : (int)RequestStatuse.Pending,
                    TimeSheets = model.TimeSheets
                };

                CommandResult result = _overtimeService.Update(criteria);
                if (result.Code == 0)
                {
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(result.Code.ToString(), result.Message);
            }

            return View(model);
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!HasPermission())
            {
                filterContext.Result = new RedirectResult("~/home/accessdenied?url=" + Request.Url.AbsoluteUri);
            }
        }

        private bool HasPermission()
        {
            var belongPMTeam = CurrentUser.Team.Code == "PM";
            var isInRoles = CurrentUser.IsInRoles(Role.AssistantLeader, Role.Leader, Role.HR, Role.SA);

            return belongPMTeam || isInRoles;
        }
    }
}
