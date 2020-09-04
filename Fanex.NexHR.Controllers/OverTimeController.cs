namespace Fanex.NexHR.Controllers
{
    using Criteria.Employee;
    using Criteria.Overtime;
    using Fanex.NexHR.Resources;
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
            const int secondCase = 2;

            switch (chosenTab)
            {
                case 1:
                    ViewBag.Tab = "NexRequest";
                    break;
                case secondCase:
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
            ViewBag.IsManager = CurrentUser.CodeOfTeam.Equals("MGT");
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
                foreach(var item in result)
                {
                    item.Own = CurrentUser.Id == item.CreatedBy.Id;
                }
            }

            return PartialView("_ListMemberOvertime", result);
        }

        [Authorize(Roles = "AssistantLeader,PM")]
        public ActionResult Create()
        {
            var model = new CreateOvertimeViewModel();

            return View(model);
        }

        [Authorize(Roles = "AssistantLeader,PM")]
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
                    TimeSheets = model.TimeSheets
                };

                CommandResult result = _commandService.Insert(criteria);
                if (result.IsSuccessful)
                {
                    model.CreatedByName = CurrentUser.DisplayName;
                    _overtimeService.SendMailForNEXRequest("NEX request Notification", model);
                    ViewBag.Inserted = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            return View(model);
        }

        [Authorize(Roles = "AssistantLeader,PM")]
        public ActionResult Edit(int id)
        {
            var criteria = new GetOvertimeByIdCriteria { Id = id };
            Overtime overtime = _overtimeService.GetOvertimeById(criteria);
            var model = new CreateOvertimeViewModel
            {
                Id = overtime.Id,
                Date = overtime.CreatedDate,
                Description = overtime.Description,
                TimeSheets = overtime.OvertimeRecords,
                CreatedByName = overtime.CreatedBy.DisplayName
            };

            model.TimeSheetData = JsonConvert.SerializeObject(
                model.TimeSheets.Select(e => new
                {
                    e.Hour,
                    Employee = new { Id = e.Employee.Id },
                    e.NexHour,
                    e.Date,
                    e.Note
                }));

            return View(model);
        }

        [Authorize(Roles = "AssistantLeader,PM")]
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
                    TimeSheets = model.TimeSheets
                };

                CommandResult result = _commandService.Update(criteria);
                if (result.IsSuccessful)
                {
                    model.CreatedByName = CurrentUser.DisplayName;
                    _overtimeService.SendMailForNEXRequest("NEX request Notification - Edit", model);
                    ViewBag.Updated = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            return View(model);
        }

        [Authorize(Roles = "AssistantLeader,PM")]
        [HttpPost]
        public JsonResult Delete(int id)
        {
            CommandResult result;
            var criteria = new DeleteOvertimeCriteria
            {
                Id = id,
                ModifiedId = CurrentUser.Id
            };
            result = _commandService.Delete(criteria);
            return Json(result);
        }

        [Authorize(Roles = "AssistantLeader,HR,PM")]
        [HttpPost]
        public ActionResult ApproveRequest(int id, int createdBy)
        {
            var criteria = new ApproveOvertimeRequestCriteria
            {
                Id = id,
                ModifiedBy = CurrentUser.Id
            };

            CommandResult result = _commandService.Update(criteria);
            if (result.IsSuccessful)
            {
                _overtimeService.SendMailForApprovalNotification("NEX request Notification", createdBy, CurrentUser.Id);
            }
            return Json(result);
        }

        [Authorize(Roles = "AssistantLeader,HR,PM")]
        [HttpPost]
        public ActionResult RejectRequest(int id)
        {
            var criteria = new RejectRequestCriteria
            {
                Id = id,
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
                ModifiedBy = CurrentUser.Id
            };

            CommandResult result = _commandService.Update(criteria);

            return Json(result);
        }

        [Authorize(Roles = "AssistantLeader,HR,PM")]
        [HttpPost]
        public ActionResult ResetRequest(int id)
        {
            var criteria = new ResetOvertimeCriteria
            {
                Id = id,
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
                    EmployeeId = model.EmployeeId,
                    OvertimeId = model.OvertimeId
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

        public ActionResult OvertimeConversionDetail(
            DateTime fromDate,
            DateTime toDate,
            int? employeeId = null,
            int? teamId = null)
        {
            return View(GetOTConversionDetailModel(fromDate, toDate, employeeId, teamId));
        }

        [HttpPost]
        public ActionResult OvertimeConversionDetail(OvertimeConversionDetailViewModel model)
        {
            IList<OvertimeConversion> conversions = new List<OvertimeConversion>();
            if (model != null)
            {
                var criteria = new GetOvertimeConversionsCriteria
                {
                    EmployeeId = model.EmployeeId <= 0 ? null : new int?(model.EmployeeId),
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    GroupId = model.TeamId
                };

                conversions = _overtimeService.GetConversions(criteria);
            }
            return PartialView("_ListOvertimeConversion", conversions);
        }

        [ChildActionOnly]
        public ActionResult CreateOvertimeConversion()
        {
            OvertimeConversionCreateViewModel model = new OvertimeConversionCreateViewModel();
            return PartialView("_CreateOvertimeConversion", model);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult CreateOvertimeConversion(OvertimeConversionCreateViewModel model)
        {
            CommandResult result;
            if (model != null && ModelState.IsValid)
            {
                var criteria = new InsertConversionCriteria
                {
                    EmpId = model.EmpId,
                    Deduct = model.Deduct,
                    DeductDate = model.DeductDate,
                    Description = string.IsNullOrEmpty(model.Description) ? string.Empty : model.Description,
                    CreatedBy = CurrentUser.Id
                };
                result = _commandService.Insert(criteria);
                if (result.IsSuccessful)
                {
                    _overtimeService.SendMailForTransferNexBalance("NEX balance Notification", model);
                }
            }
            else
            {
                result = new CommandResult
                {
                    Code = 1,
                    Message = Messages.InvalidInputData
                };
            }
            return Json(result);
        }

        [ChildActionOnly]
        public ActionResult EditOvertimeConversion()
        {
            OvertimeConversionEditViewModel model = new OvertimeConversionEditViewModel();
            return PartialView("_EditOvertimeConversion", model);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult EditOvertimeConversion(OvertimeConversionEditViewModel model)
        {
            CommandResult result;
            if (model != null && ModelState.IsValid)
            {
                var criteria = new UpdateConversionCriteria
                {
                    Id = model.Id,
                    Deduct = model.Deduct,
                    DeductDate = model.DeductDate,
                    Description = string.IsNullOrEmpty(model.Description) ? string.Empty : model.Description,
                    ModifiedBy = CurrentUser.Id
                };
                result = _commandService.Update(criteria);
            }
            else
            {
                result = new CommandResult
                {
                    Code = 1,
                    Message = Messages.InvalidInputData
                };
            }
            return Json(result);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult DeleteOvertimeConversion(int id)
        {
            CommandResult result;
            var criteria = new DeleteConversionCriteria
            {
                Id = id,
                ModifiedBy = CurrentUser.Id
            };
            result = _commandService.Delete(criteria);
            return Json(result);
        }

        private OvertimeConversionDetailViewModel GetOTConversionDetailModel(
            DateTime fromDate, 
            DateTime toDate, 
            int? employeeId, 
            int? teamId)
        {
            IList<Team> teams = _teamService.GetTeam();

            var model = new OvertimeConversionDetailViewModel
            {
                EmployeeId = employeeId ?? -1,
                FromDate = fromDate,
                ToDate = toDate,
                TeamId = teamId,
                Teams = teams
            };

            if (employeeId.HasValue)
            {
                var getEmployeeCriteria = new GetEmployeeByIdCriteria { EmployeeId = employeeId.Value };
                Employee employee = _employeeService.GetEmployeeById(getEmployeeCriteria);

                model.Employee = employee.DisplayName;
            }

            return model;
        }

        [Authorize(Roles = "AssistantLeader,PM")]
        [HttpPost]
        public ActionResult CreateEmployeeRecord(OvertimeRecord model, int index)
        {
            ViewData["index"] = index;
            var criteria = new GetNexHourCriteria
            {
                EmployeeId = model.Employee.Id,
                Date = model.Date,
                Hour = model.Hour
            };
            var nexHour = _overtimeService.GetNexHour(criteria);
            model.NexHour = nexHour.Value;
            return PartialView("_CreateEmployeeRecord", model);
        }


        [Authorize(Roles = "AssistantLeader,HR,PM")]
        public JsonResult GetNexHour(int employeeId, DateTime date, float hour)
        {
            var criteria = new GetNexHourCriteria
            {
                EmployeeId = employeeId,
                Date = date,
                Hour = hour
            };
            var nexHour = _overtimeService.GetNexHour(criteria);
            return Json(nexHour, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "HR")]
        public ActionResult HRView()
        {
            var model = new HROvertimeViewModel();
            return View(model);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public ActionResult HRView(HROvertimeViewModel model)
        {
            IList<Overtime> result = new List<Overtime>();
            if (model != null)
            {
                var criteria = new ReportOvertimeHRCriteria
                {
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    CreatedBy = model.CreatedBy,
                    Status = model.Status
                };
                result = _overtimeService.GetReportOvertimeHR(criteria);
                foreach (var item in result)
                {
                    item.Own = CurrentUser.Id == item.CreatedBy.Id;
                }
            }

            return PartialView("_ListHRViewOvertime", result);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public ActionResult HRViewDetail(int id)
        {
            var criteria = new ReportOvertimeHRDetailCriteria
            {
                Id = id
            };
            var result = _overtimeService.GetReportOvertimeHRDetail(criteria);
            return PartialView("_ListHRViewOvertimeDetail", result);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public ActionResult VoidRequest(int id)
        {
            var criteria = new VoidOvertimeCriteria
            {
                Id = id,
                ModifiedBy = CurrentUser.Id
            };

            CommandResult result = _commandService.Update(criteria);

            return Json(result);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public ActionResult VerifyRequest(OvertimeVerifyViewModel model)
        {
            model.TimeSheets = JsonConvert.DeserializeObject<IList<OvertimeRecord>>(model.TimeSheetData);
            var criteria = new VerifyOvertimeCriteria
            {
                ModifiedBy = CurrentUser.Id,
                Id = model.Id,
                TimeSheets = model.TimeSheets
            };

            CommandResult result = _commandService.Update(criteria);
            if (result.IsSuccessful)
            {
                _overtimeService.SendMailForNotification("NEX request Notification", model);
            }
            return Json(result);
        }
    }
}
