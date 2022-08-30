namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Criteria.Employee;
    using Criteria.Overtime;
    using Fanex.NexHR.Controllers.Attributes;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Resources;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.Overtime;
    using Newtonsoft.Json;
    using Services.Interfaces;
    using ViewModels.Overtime;

    public class OvertimeController : BaseController
    {
        private readonly IOvertimeService overtimeService;
        private readonly IEmployeeService employeeService;
        private readonly IBaseService baseService;
        private readonly ITeamService teamService;

        public OvertimeController(ISessionService sessionService,
            IOvertimeService overtimeService,
            IEmployeeService employeeService,
            IBaseService baseService,
            ITeamService teamService) : base(sessionService)
        {
            this.overtimeService = overtimeService;
            this.employeeService = employeeService;
            this.teamService = teamService;
            this.baseService = baseService;
        }

        public IActionResult Index(int chosenTab = 0)
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

            var userRoles = employeeService.UserRoles(CurrentUser);
            ViewBag.ValidRole = userRoles.Any(item => (item == Role.HR.EnumToString() || item == Role.PM.EnumToString() || item == Role.AssistantLeader.EnumToString()));

            return View();
        }

        [HrmAuthorize(Roles = "AssistantLeader,HR,PM")]
        public IActionResult MemberOvertime()
        {
            var model = new MemberOvertimeViewModel();
            ViewBag.IsManager = CurrentUser.CodeOfTeam.Equals("MGT");
            return PartialView(model);
        }

        [HrmAuthorize(Roles = "AssistantLeader,HR,PM")]
        [HttpPost]
        public async Task<IActionResult> MemberOvertimeData(MemberOvertimeViewModel model)
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
                result = await overtimeService.GetOvertime(criteria);
                foreach (var item in result)
                {
                    item.Own = CurrentUser.Id == item.CreatedBy.Id;
                }
            }
            ViewBag.IsManager = CurrentUser.CodeOfTeam.Equals("MGT");
            return PartialView("_ListMemberOvertime", result);
        }

        [HrmAuthorize(Roles = "AssistantLeader,PM")]
        public IActionResult Create()
        {
            var model = new CreateOvertimeViewModel();

            return View(model);
        }

        [HrmAuthorize(Roles = "AssistantLeader,PM")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOvertimeViewModel model)
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

                CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);
                if (result.IsSuccessful)
                {
                    model.CreatedByName = CurrentUser.DisplayName;
                    await overtimeService.SendMailForNEXRequest("NEX request Notification", model);
                    ViewBag.Inserted = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            return View(model);
        }

        [HrmAuthorize(Roles = "AssistantLeader,PM")]
        public async Task<IActionResult> Edit(int id)
        {
            var criteria = new GetOvertimeByIdCriteria { Id = id };
            Overtime overtime = await overtimeService.GetOvertimeById(criteria);
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

        [HrmAuthorize(Roles = "AssistantLeader,PM")]
        [HttpPost]
        public async Task<IActionResult> Edit(CreateOvertimeViewModel model)
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

                CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);
                if (result.IsSuccessful)
                {
                    model.CreatedByName = CurrentUser.DisplayName;
                    await overtimeService.SendMailForNEXRequest("NEX request Notification - Edit", model);
                    ViewBag.Updated = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            return View(model);
        }

        [HrmAuthorize(Roles = "AssistantLeader,PM")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            CommandResult result;
            var criteria = new DeleteOvertimeCriteria
            {
                Id = id,
                ModifiedId = CurrentUser.Id
            };
            result = await baseService.ExecuteNonQueryAsync(criteria);
            return Json(result);
        }

        [HrmAuthorize(Roles = "AssistantLeader,HR,PM")]
        [HttpPost]
        public async Task<IActionResult> ApproveRequest(int id, int createdBy)
        {
            var criteria = new ApproveOvertimeRequestCriteria
            {
                Id = id,
                ModifiedBy = CurrentUser.Id
            };

            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);
            if (result.IsSuccessful)
            {
                await overtimeService.SendMailForApprovalNotification("NEX request Notification", id, createdBy, CurrentUser.Id);
            }
            return Json(result);
        }

        [HrmAuthorize(Roles = "AssistantLeader,HR,PM")]
        [HttpPost]
        public async Task<IActionResult> RejectRequest(int id)
        {
            var criteria = new RejectRequestCriteria
            {
                Id = id,
                ModifiedBy = CurrentUser.Id
            };

            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);

            return Json(result);
        }

        [HrmAuthorize(Roles = "AssistantLeader,HR,PM")]
        [HttpPost]
        public async Task<IActionResult> DisapproveRequest(int id)
        {
            var criteria = new DisapproveOvertimeRequestCriteria
            {
                Id = id,
                ModifiedBy = CurrentUser.Id
            };

            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);

            return Json(result);
        }

        [HrmAuthorize(Roles = "AssistantLeader,HR,PM")]
        [HttpPost]
        public async Task<IActionResult> ResetRequest(int id)
        {
            var criteria = new ResetOvertimeCriteria
            {
                Id = id,
                ModifiedBy = CurrentUser.Id
            };

            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);

            return Json(result);
        }

        public IActionResult MyOvertime()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> MyOvertimeData(int year)
        {
            var criteria = new GetMyOvertimeCriteria
            {
                EmployeeId = CurrentUser.Id,
                FromDate = new DateTime(year, 1, 1),
                ToDate = new DateTime(year, 12, 31)
            };
            IList<EmployeeOvertime> employeeOvertimes = await overtimeService.GetMyOvertime(criteria);

            return PartialView("_ListMyOvertime", employeeOvertimes);
        }

        public async Task<IActionResult> OvertimeRequest(MemberOvertimeViewModel model)
        {
            if (model != null)
            {
                if (model.EmployeeId.HasValue)
                {
                    var criteria = new GetEmployeeByIdCriteria { EmployeeId = model.EmployeeId.Value };
                    var employee = await employeeService.GetEmployeeById(criteria);
                    model.Employee = employee.DisplayName;
                }

                if (model.CreatedBy.HasValue)
                {
                    var criteria = new GetEmployeeByIdCriteria { EmployeeId = model.CreatedBy.Value };
                    var employee = await employeeService.GetEmployeeById(criteria);
                    model.CreatedName = employee.DisplayName;
                }
            }

            return PartialView(model);
        }

        [HttpPost]
        public async Task<IActionResult> OvertimeRequestDetail(MemberOvertimeViewModel model)
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
                result = await overtimeService.GetEmployeeOvertime(criteria);
            }

            return PartialView("_ListOvertimeRequest", result);
        }

        public async Task<IActionResult> OvertimeSummary()
        {
            OvertimeSummaryViewModel summary = new OvertimeSummaryViewModel
            {
                Teams = await teamService.GetTeam(),
                Year = DateTime.Now.Year
            };

            return View(summary);
        }

        [HttpPost]
        public async Task<IActionResult> OvertimeSummary(int? empId, int? teamId, int year)
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

            IList<OvertimeSummary> listSummary = await overtimeService.GetOvertimeSummary(criteria);
            ViewBag.Year = year;
            return PartialView("_ListOvertimeSummary", listSummary);
        }

        public async Task<IActionResult> OvertimeConversionDetail(
            DateTime fromDate,
            DateTime toDate,
            int? employeeId = null,
            int? teamId = null)
        {
            var viewModel = await GetOTConversionDetailModel(fromDate, toDate, employeeId, teamId);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> OvertimeConversionDetail(OvertimeConversionDetailViewModel model)
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

                conversions = await overtimeService.GetConversions(criteria);
            }
            return PartialView("_ListOvertimeConversion", conversions);
        }

        public IActionResult CreateOvertimeConversion()
        {
            OvertimeConversionCreateViewModel model = new OvertimeConversionCreateViewModel();
            return PartialView("_CreateOvertimeConversion", model);
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public async Task<IActionResult> CreateOvertimeConversion(OvertimeConversionCreateViewModel model)
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
                result = await baseService.ExecuteNonQueryAsync(criteria);
                if (result.IsSuccessful)
                {
                    await overtimeService.SendMailForTransferNexBalance("NEX balance Notification", model);
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

        public IActionResult EditOvertimeConversion()
        {
            OvertimeConversionEditViewModel model = new OvertimeConversionEditViewModel();
            return PartialView("_EditOvertimeConversion", model);
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public async Task<IActionResult> EditOvertimeConversion(OvertimeConversionEditViewModel model)
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
                result = await baseService.ExecuteNonQueryAsync(criteria);
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

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public async Task<IActionResult> DeleteOvertimeConversion(int id)
        {
            CommandResult result;
            var criteria = new DeleteConversionCriteria
            {
                Id = id,
                ModifiedBy = CurrentUser.Id
            };
            result = await baseService.ExecuteNonQueryAsync(criteria);
            return Json(result);
        }

        private async Task<OvertimeConversionDetailViewModel> GetOTConversionDetailModel(
            DateTime fromDate,
            DateTime toDate,
            int? employeeId,
            int? teamId)
        {
            IList<Team> teams = await teamService.GetTeam();

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
                Employee employee = await employeeService.GetEmployeeById(getEmployeeCriteria);

                model.Employee = employee.DisplayName;
            }

            return model;
        }

        [HrmAuthorize(Roles = "AssistantLeader,PM")]
        [HttpPost]
        public async Task<IActionResult> CreateEmployeeRecord(OvertimeRecord model, int index)
        {
            ViewData["index"] = index;
            var criteria = new GetNexHourCriteria
            {
                EmployeeId = model.Employee.Id,
                Date = model.Date,
                Hour = model.Hour
            };
            var nexHour = await overtimeService.GetNexHour(criteria);
            model.NexHour = nexHour.Value;
            return PartialView("_CreateEmployeeRecord", model);
        }

        [HrmAuthorize(Roles = "AssistantLeader,HR,PM")]
        public async Task<IActionResult> GetNexHour(int employeeId, DateTime date, float hour)
        {
            var criteria = new GetNexHourCriteria
            {
                EmployeeId = employeeId,
                Date = date,
                Hour = hour
            };
            var nexHour = await overtimeService.GetNexHour(criteria);
            return Json(nexHour);
        }

        [HrmAuthorize(Roles = "HR")]
        public IActionResult HRView()
        {
            var model = new HROvertimeViewModel();
            return View(model);
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public async Task<IActionResult> HRView(HROvertimeViewModel model)
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
                result = await overtimeService.GetReportOvertimeHR(criteria);
                foreach (var item in result)
                {
                    item.Own = CurrentUser.Id == item.CreatedBy.Id;
                }
            }

            return PartialView("_ListHRViewOvertime", result);
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public async Task<IActionResult> HRViewDetail(int id)
        {
            var criteria = new ReportOvertimeHRDetailCriteria
            {
                Id = id
            };
            var result = await overtimeService.GetReportOvertimeHRDetail(criteria);
            return PartialView("_ListHRViewOvertimeDetail", result);
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public async Task<IActionResult> VoidRequest(int id)
        {
            var criteria = new VoidOvertimeCriteria
            {
                Id = id,
                ModifiedBy = CurrentUser.Id
            };

            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);

            return Json(result);
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public async Task<IActionResult> VerifyRequest(OvertimeVerifyViewModel model)
        {
            model.TimeSheets = JsonConvert.DeserializeObject<IList<OvertimeRecord>>(model.TimeSheetData);
            var criteria = new VerifyOvertimeCriteria
            {
                ModifiedBy = CurrentUser.Id,
                Id = model.Id,
                TimeSheets = model.TimeSheets
            };

            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);
            if (result.IsSuccessful)
            {
                await overtimeService.SendMailForNotification("NEX request Notification", model);
            }
            return Json(result);
        }

        public async Task<IActionResult> OvertimeNexDetail(DateTime fromDate, DateTime toDate, int? employeeId)
        {
            var model = new MemberOvertimeViewModel
            {
                EmployeeId = employeeId,
                FromDateOT = fromDate,
                ToDateOT = toDate,
            };

            if (employeeId.HasValue)
            {
                var criteria = new GetEmployeeByIdCriteria { EmployeeId = employeeId.Value };
                var employee = await employeeService.GetEmployeeById(criteria);
                model.Employee = employee.DisplayName;
            }

            return View("OvertimeRequestDetail", model);
        }
    }
}