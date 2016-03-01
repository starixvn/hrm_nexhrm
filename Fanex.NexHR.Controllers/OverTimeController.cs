namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Overtime;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.ViewModels.Overtime;
    using Newtonsoft.Json;
    using Services;
    using Services.Interfaces;

    public class OvertimeController : BaseController
    {
        private readonly IOvertimeService overtimeService;
        private readonly IEmployeeService employeeService;
        private readonly ICommandService commandService;

        public OvertimeController()
            : this(null, null, null)
        {
        }

        public OvertimeController(
            IOvertimeService service,
            IEmployeeService employeeService,
            ICommandService commandService)
        {
            this.overtimeService = service ?? new OvertimeService();
            this.employeeService = employeeService ?? new EmployeeService();
            this.commandService = commandService ?? new CommandService();
        }

        public ActionResult MemberOvertime()
        {
            var model = new MemberOvertimeViewModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult MemberOvertimeData(MemberOvertimeViewModel model)
        {
            IList<Overtime> result = new List<Overtime>();
            if (model != null)
            {
                var criteria = new GetOvertimeCriteria
                {
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    CreatedBy = model.CreatedBy,
                    Status = model.Status
                };
                result = overtimeService.GetOvertime(criteria);
            }

            return View(result);
        }

        public ActionResult Create()
        {
            var model = new CreateOvertimeViewModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateOvertimeViewModel model)
        {
            if (model != null)
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

                    CommandResult result = commandService.Insert(criteria);
                    if (result.IsSuccessful)
                    {
                        ViewBag.Inserted = true;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Message);
                    }
                }
            }

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var criteria = new GetOvertimeByIdCriteria { Id = id };
            Overtime overtime = overtimeService.GetOvertimeById(criteria);
            var model = new CreateOvertimeViewModel()
            {
                Id = overtime.Id,
                Date = overtime.OnDate,
                Description = overtime.Description,
                TimeSheets = overtime.OvertimeRecords,
                CreatedByName = overtime.CreatedBy.DisplayName
            };

            model.TimeSheetData = JsonConvert.SerializeObject(
                model.TimeSheets.Select(e => new
                {
                    e.Hour,
                    Employee = new { Id = e.Employee.Id }
                }));

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(CreateOvertimeViewModel model)
        {
            if (model != null && ModelState.IsValid && HasPermission())
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

                CommandResult result = commandService.Update(criteria);
                if (result.IsSuccessful)
                {
                    ViewBag.Updated = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
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

            CommandResult result = commandService.Update(criteria);

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

            CommandResult result = commandService.Update(criteria);

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

            CommandResult result = commandService.Update(criteria);

            return Json(result);
        }

        public ActionResult MyOvertime()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MyOvertimeData(int year)
        {
            var criteria = new GetMyOvertimeCriteria
            {
                EmployeeId = CurrentUser.Id,
                FromDate = new DateTime(year, 1, 1),
                ToDate = new DateTime(year, 12, 31)
            };
            IList<EmployeeOvertime> employeeOvertimes = overtimeService.GetMyOvertime(criteria);

            return View(employeeOvertimes);
        }

        public ActionResult OvertimeRequest(MemberOvertimeViewModel model)
        {
            if (model != null)
            {
                if (model.EmployeeId.HasValue)
                {
                    var criteria = new GetEmployeeByIdCriteria { EmployeeId = model.EmployeeId.Value };
                    model.Employee = employeeService.GetEmployeeById(criteria).DisplayName;
                }

                if (model.CreatedBy.HasValue)
                {
                    var criteria = new GetEmployeeByIdCriteria { EmployeeId = model.CreatedBy.Value };
                    model.CreatedName = employeeService.GetEmployeeById(criteria).DisplayName;
                }
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult OvertimeRequestDetail(MemberOvertimeViewModel model)
        {
            IList<EmployeeOvertime> result = new List<EmployeeOvertime>();
            if (model != null)
            {
                var criteria = new GetEmployeeOvertimeCriteria
                {
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    CreatedBy = model.CreatedBy,
                    Status = model.Status,
                    EmployeeId = model.EmployeeId
                };
                result = overtimeService.GetEmployeeOvertime(criteria);
            }

            return View(result);
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext != null && !HasPermission())
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
