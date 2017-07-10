namespace Fanex.NexHR.Controllers
{
    using Core.Enums;
    using Criteria.Employee;
    using Criteria.Overtime;
    using Models;
    using Models.Overtime;
    using Newtonsoft.Json;
    using Services;
    using Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using ViewModels.Overtime;

    public class OvertimeController : BaseController
    {
        private readonly IOvertimeService _overtimeService;
        private readonly IEmployeeService _employeeService;
        private readonly ICommandService _commandService;
        private readonly ITeamService _teamService;

        public OvertimeController()
            : this(null, null, null, null)
        {
        }

        public OvertimeController(
            IOvertimeService service,
            IEmployeeService employeeService,
            ICommandService commandService,
            ITeamService teamService)
        {
            _overtimeService = service ?? new OvertimeService();
            _employeeService = employeeService ?? new EmployeeService();
            _commandService = commandService ?? new CommandService();
            _teamService = teamService ?? new TeamService();
        }

        public ActionResult Index(int chosenTab = 0)
        {
            switch (chosenTab)
            {
                case 1:
                    ViewBag.Tab = "NexRequest";
                    break;
                case 2:
                    ViewBag.Tab = "NexDetail";
                    break;
                default:
                    ViewBag.Tab = "MyNex";
                    break;
            }

            return View();
        }

        [Authorize(Roles = "AssistantLeader,HR,PM")]
        public ActionResult MemberOvertime()
        {
            var model = new MemberOvertimeViewModel();
            return PartialView(model);
        }

        [ValidateInput(false)]
        [Authorize(Roles = "AssistantLeader,HR,PM")]
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
                result = _overtimeService.GetOvertime(criteria);
            }

            return PartialView("_ListMemberOvertime", result);
        }

        [Authorize(Roles = "AssistantLeader,HR,PM")]
        public ActionResult Create()
        {
            var model = new CreateOvertimeViewModel();

            return View(model);
        }

        [Authorize(Roles = "AssistantLeader,HR,PM")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateOvertimeViewModel model)
        {
            if (model != null && ModelState.IsValid)
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

                CommandResult result = _commandService.Insert(criteria);
                if (result.IsSuccessful)
                {
                    _overtimeService.SendMail("NEX request Notification", model);
                    ViewBag.Inserted = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            return View(model);
        }

        [Authorize(Roles = "AssistantLeader,HR,PM")]
        public ActionResult Edit(int id)
        {
            var criteria = new GetOvertimeByIdCriteria { Id = id };
            Overtime overtime = _overtimeService.GetOvertimeById(criteria);
            var model = new CreateOvertimeViewModel
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

        [Authorize(Roles = "AssistantLeader,HR,PM")]
        [HttpPost]
        public ActionResult Edit(CreateOvertimeViewModel model)
        {
            if (model != null && ModelState.IsValid)
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

                CommandResult result = _commandService.Update(criteria);
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

        [Authorize(Roles = "AssistantLeader,HR,PM")]
        [HttpPost]
        public ActionResult ApproveRequest(int id)
        {
            var criteria = new ApproveOvertimeRequestCriteria
            {
                Id = id,
                Status = (int)RequestStatuse.Approve,
                ModifiedBy = CurrentUser.Id
            };

            CommandResult result = _commandService.Update(criteria);

            return Json(result);
        }

        [Authorize(Roles = "AssistantLeader,HR,PM")]
        [HttpPost]
        public ActionResult RejectRequest(int id)
        {
            var criteria = new RejectRequestCriteria
            {
                Id = id,
                Status = (int)RequestStatuse.Reject,
                ModifiedBy = CurrentUser.Id
            };

            CommandResult result = _commandService.Update(criteria);

            return Json(result);
        }

        [Authorize(Roles = "AssistantLeader,HR,PM")]
        [HttpPost]
        public ActionResult DisapproveRequest(int id)
        {
            var criteria = new DisapproveOvertimeRequestCriteria
            {
                Id = id,
                Status = (int)RequestStatuse.Pending,
                ModifiedBy = CurrentUser.Id
            };

            CommandResult result = _commandService.Update(criteria);

            return Json(result);
        }

        public ActionResult MyOvertime()
        {
            return PartialView();
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
            IList<EmployeeOvertime> employeeOvertimes = _overtimeService.GetMyOvertime(criteria);

            return PartialView("_ListMyOvertime", employeeOvertimes);
        }

        public ActionResult OvertimeRequest(MemberOvertimeViewModel model)
        {
            if (model != null)
            {
                if (model.EmployeeId.HasValue)
                {
                    var criteria = new GetEmployeeByIdCriteria { EmployeeId = model.EmployeeId.Value };
                    model.Employee = _employeeService.GetEmployeeById(criteria).DisplayName;
                }

                if (model.CreatedBy.HasValue)
                {
                    var criteria = new GetEmployeeByIdCriteria { EmployeeId = model.CreatedBy.Value };
                    model.CreatedName = _employeeService.GetEmployeeById(criteria).DisplayName;
                }
            }

            return PartialView(model);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult OvertimeRequestDetail(MemberOvertimeViewModel model)
        {
            IList<EmployeeOvertime> result = new List<EmployeeOvertime>();
            if (model != null)
            {
                var criteria = new GetEmployeeOvertimeCriteria
                {
                    FromDate = model.FromDateOT,
                    ToDate = model.ToDateOT,
                    CreatedBy = model.CreatedBy,
                    Status = model.Status,
                    EmployeeId = model.EmployeeId
                };
                result = _overtimeService.GetEmployeeOvertime(criteria);
            }

            return PartialView("_ListOvertimeRequest", result);
        }

        public ActionResult OvertimeSummary()
        {
            OvertimeSummaryViewModel summary = new OvertimeSummaryViewModel
            {
                Teams = _teamService.GetTeam(),
                Year = DateTime.Now.Year
            };

            return View(summary);
        }

        [HttpPost]
        public ActionResult OvertimeSummary(int? empId, int? teamId, int year)
        {
            int? employeeId = null;
            if (empId.HasValue && empId.Value >= 0)
            {
                employeeId = empId;
            }

            OvertimeSummaryCriteria criteria = new OvertimeSummaryCriteria
            {
                EmployeeId = employeeId,
                TeamId = teamId,
                Year = year
            };

            List<OvertimeSummary> listSummary = _overtimeService.GetOvertimeSummary(criteria);
            ViewBag.Year = year;
            return PartialView("_ListOvertimeSummary", listSummary);
        }
    }
}
