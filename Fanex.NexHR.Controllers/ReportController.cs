// --------------------------------------------------------------------------
// <copyright file="ReportController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
//   <change who="Josh.Phan" date="2014.03.11">Apply NAP Framework</change>
// </history>
namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Security;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.ViewModels.Report;

    [Authorize]
    public class ReportController : BaseController
    {
        private readonly IEmployeeService _employeeService;
        private readonly IReportService _reportService;
        private readonly ITeamService _teamService;

        public ReportController(
            IEmployeeService employeeService = null,
            IReportService reportService = null,
            ITeamService teamService = null)
        {
            _employeeService = employeeService ?? new EmployeeService();
            _reportService = reportService ?? new ReportService();
            _teamService = teamService ?? new TeamService();
        }

        public ReportController()
            : this(null, null, null)
        {
        }

        public ActionResult Attendance(EmployeeAttendanceViewModel model)
        {
            var employee = this.CurrentUser;
            ViewBag.Groups = _teamService.GetAll(new GetAllTeamCriteria());

            if (model.FromDate == DateTime.MinValue)
            {
                model = new EmployeeAttendanceViewModel
                {
                    FromDate = DateTime.Now,
                    ToDate = DateTime.Now,
                    GroupId = employee.Team.Id,
                    DateOption = 0,
                    ExcludeUserDelete = true,
                };
            }

            return View(model);
        }

        public ActionResult Timesheet()
        {
            var model = new TimesheetViewModel
            {
                Year = DateTime.Now.Year,
                EmpId = CurrentUser.Id
            };
            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult HRRequestMonitoring()
        {
            return HRRequestMonitoring(null);
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult HRRequestMonitoring(RequestsMonitoringViewModel model)
        {
            if (model == null)
            {
                var year = DateTime.Now.Year;
                var month = DateTime.Now.Month;
                model = new RequestsMonitoringViewModel
                {
                    FromDate = new DateTime(year, month, 1),
                    ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                    MaxRequest = 2,
                    DateOption = 0
                };
            }

            ViewBag.FromDate = model.FromDate;
            ViewBag.ToDate = model.ToDate;
            ViewBag.isLeader = false;

            IList<RequestsMonitoring> requestMonitorings = new List<RequestsMonitoring>();

            IList<short> roleList = this.CurrentUser.RoleList;
            if (roleList != null)
            {
                if (roleList.Any(x => x == (int)Role.HR || x == (int)Role.SA))
                {
                    var getRequestMonitorCriteria = new GetRequestMonitorReportCriteria
                    {
                        FromDate = model.FromDate,
                        ToDate = model.ToDate,
                        MaxRequest = model.MaxRequest,
                        Role = (int)Role.HR,
                        EmployeeId = this.CurrentUser.Id
                    };

                    requestMonitorings = _reportService.GetRequestMonitoring(getRequestMonitorCriteria);
                }
            }

            ViewBag.EntityList = requestMonitorings;

            return View("RequestMonitoring", model);
        }

        [Authorize(Roles = "AssistantLeader")]
        public ActionResult LeaderRequestMonitoring()
        {
            return LeaderRequestMonitoring(null);
        }

        [HttpPost]
        [Authorize(Roles = "AssistantLeader")]
        public ActionResult LeaderRequestMonitoring(RequestsMonitoringViewModel model)
        {
            if (model == null)
            {
                var year = DateTime.Now.Year;
                var month = DateTime.Now.Month;
                model = new RequestsMonitoringViewModel
                {
                    FromDate = new DateTime(year, month, 1),
                    ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                    MaxRequest = 1,
                    DateOption = 0
                };
            }

            ViewBag.FromDate = model.FromDate;
            ViewBag.ToDate = model.ToDate;
            ViewBag.isLeader = true;
            ViewBag.EntityList = new List<RequestsMonitoring>();

            IList<short> roleList = this.CurrentUser.RoleList;
            if (roleList != null)
            {
                if (roleList.Any(x => x == (int)Role.AssistantLeader || x == (int)Role.Leader || x == (int)Role.SA))
                {
                    var getRequestMonitorCriteria = new GetRequestMonitorReportCriteria
                    {
                        FromDate = model.FromDate,
                        ToDate = model.ToDate,
                        MaxRequest = model.MaxRequest,
                        Role = (int)Role.AssistantLeader,
                        EmployeeId = this.CurrentUser.Id
                    };

                    ViewBag.EntityList = _reportService.GetRequestMonitoring(getRequestMonitorCriteria);
                }
            }

            return View("RequestMonitoring", model);
        }

        public ActionResult TotalWorkingHours()
        {
            return TotalWorkingHours(null);
        }

        [HttpPost]
        public ActionResult TotalWorkingHours(TotalWorkingHoursViewModel model)
        {
            if (model == null)
            {
                var year = DateTime.Now.Year;
                var month = DateTime.Now.Month;
                model = new TotalWorkingHoursViewModel
                {
                    FromDate = new DateTime(year, month, 1),
                    ToDate = DateTime.Now,
                    DateOption = 5,
                    IsExportExcel = false
                };
            }

            if (model.IsExportExcel)
            {
                var export = new ExportController();
                return export.TotalWorkingHours(model);
            }

            ViewBag.Groups = _teamService.GetAll(new GetAllTeamCriteria());
            ViewBag.TeamId = model.TeamId;
            var getWorkingHoursCriteria = new GetTotalWorkingHoursSummaryCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                EmployeeId = (model.EmployeeId != -1 & model.EmployeeId != 0) ? new int?(model.EmployeeId) : null,
                GroupId = model.TeamId
            };

            var totalWorkingHours = _reportService.GetTotalWorkingHours(getWorkingHoursCriteria);

            ViewBag.EntityList = totalWorkingHours;

            return View(model);
        }

        public ActionResult LeaveSummary()
        {
            return LeaveSummary(null);
        }

        [HttpPost]
        public ActionResult LeaveSummary(LeaveSummaryViewModel model)
        {
            if (model == null)
            {
                model = new LeaveSummaryViewModel
                {
                    Year = DateTime.Now.Year,
                    EmployeeId = -1
                };
            }

            var getAnnualLeaveCriteria = new GetAnnualLeaveSummaryReportCriteria
            {
                EmployeeId = model.EmployeeId != -1 ? new int?(model.EmployeeId) : null,
                GroupId = model.TeamId,
                Year = model.Year
            };
            IList<LeaveSummary> entityList = _reportService.GetAnnualLeaveBalanceReport(getAnnualLeaveCriteria);

            IList<Team> teams = new List<Team>();

            if (model.TeamId != null)
            {
                var criteria = new GetTeamByIdCriteria
                {
                    Id = (int)model.TeamId
                };
                Team team = _teamService.GetById(criteria);
                teams.Add(team);
            }
            else
            {
                teams = _teamService.GetAll(new GetAllTeamCriteria());
            }

            ViewBag.Groups = _teamService.GetAll(new GetAllTeamCriteria());
            ViewBag.TeamId = model.TeamId;
            ViewBag.EntityList = entityList;

            IList<LeaveSummaryByTeam> leaveSummaryByTeamList = _employeeService.GetAnnualLeaveByTeam(entityList, teams);

            ViewBag.LeaveSummaryByTeamList = leaveSummaryByTeamList;

            return View(model);
        }

        [ChildActionOnly]
        public PartialViewResult CreateLeaveDeduction()
        {
            return PartialView(new LeaveDeductionViewModel
            {
                DeductDate = DateTime.Now,
                TotalDays = decimal.Parse("0.5"),
                Description = Labels.LeaveDeductionDescription,
                IsAnnual = true
            });
        }

        [ChildActionOnly]
        public PartialViewResult EditLeaveDeduction()
        {
            return PartialView(new LeaveDeductionViewModel
            {
                DeductDate = DateTime.Now,
                TotalDays = decimal.Parse("0.5"),
                Description = Labels.LeaveDeductionDescription,
                IsAnnual = true
            });
        }

        public ActionResult LateInEarlyOutSummary()
        {
            return LateInEarlyOutSummary(null);
        }

        [HttpPost]
        public ActionResult LateInEarlyOutSummary(LateInEarlyOutSummaryViewModel model)
        {
            IList<Team> teams = _teamService.GetAll(new GetAllTeamCriteria());
            if (model == null)
            {
                model = new LateInEarlyOutSummaryViewModel()
                {
                    Year = DateTime.Now.Year
                };
            }

            model.Groups = teams;
            var criteria = new GetLateInEarlyOutSummaryReportCriteria
            {
                EmployeeId = model.EmployeeId > 0 ? new int?(model.EmployeeId) : null,
                GroupId = model.TeamId,
                Year = model.Year
            };
            IList<LateInEarlyOutSummary> lateInEarlyOuts = _reportService.GetLateInEarlyOutSummary(criteria);
            if (model.TeamId.HasValue)
            {
                teams = teams.Where(t => t.Id == model.TeamId.Value).ToList();
            }

            IList<LateInEarlyOutSummaryByTeam> lateInEarlyOutSummaryByTeam = _reportService.GetLateInEarlyOutSummaryByTeam(lateInEarlyOuts, teams);
            model.LateInEarlyOutSummary = lateInEarlyOutSummaryByTeam;

            return View(model);
        }

        public ActionResult LateInEarlyOutDetail(DateTime fromDate, DateTime toDate, int? employeeId = null, int? teamId = null)
        {
            var model = new LateInEarlyOutDetailViewModel
            {
                EmployeeId = employeeId.HasValue ? employeeId.Value : -1,
                FromDate = fromDate,
                ToDate = toDate,
                IncludeLeaveDeduction = false,
                TeamId = teamId
            };

            if (employeeId.HasValue)
            {
                var getEmployeeCriteria = new GetEmployeeCriteria() { Id = employeeId.Value };
                var employee = _employeeService.GetEmployeeById(getEmployeeCriteria);
                model.Employee = employee.DisplayName;
            }

            return LateInEarlyOutDetail(model);
        }

        [HttpPost]
        public ActionResult LateInEarlyOutDetail(LateInEarlyOutDetailViewModel model)
        {
            IList<Team> teams = _teamService.GetAll(new GetAllTeamCriteria());
            model.Teams = teams;

            var criteria = new GetLateInEarlyOutDetailReportCriteria()
            {
                EmployeeId = model.EmployeeId == -1 ? null : new int?(model.EmployeeId),
                GroupId = model.TeamId,
                IncludeLeaveDeduction = model.IncludeLeaveDeduction,
                FromDate = model.FromDate,
                ToDate = model.ToDate
            };

            var lateInEarlyOuts = _reportService.GetLateInEarlyOutDetail(criteria);
            model.LateInEarlyOuts = lateInEarlyOuts;

            return View(model);
        }

        public ActionResult LeaveDeductionDetail(DateTime fromDate, DateTime toDate, int? employeeId = null, int? teamId = null)
        {
            var model = new LeaveDeductionDetailViewModel
            {
                EmployeeId = employeeId.HasValue ? employeeId.Value : -1,
                FromDate = fromDate,
                ToDate = toDate,
                TeamId = teamId
            };

            if (employeeId.HasValue)
            {
                var getEmployeeCriteria = new GetEmployeeCriteria() { Id = employeeId.Value };
                var employee = _employeeService.GetEmployeeById(getEmployeeCriteria);
                model.Employee = employee.DisplayName;
            }

            return LeaveDeductionDetail(model);
        }

        [HttpPost]
        public ActionResult LeaveDeductionDetail(LeaveDeductionDetailViewModel model)
        {
            IList<Team> teams = _teamService.GetAll(new GetAllTeamCriteria());
            model.Teams = teams;

            var criteria = new GetLeaveDeductionDetailCriteria()
            {
                EmployeeId = model.EmployeeId == -1 ? null : new int?(model.EmployeeId),
                GroupId = model.TeamId,
                FromDate = model.FromDate,
                ToDate = model.ToDate
            };

            var leaveDeductions = _reportService.GetLeaveDeductionDetail(criteria);
            model.LeaveDeductions = leaveDeductions;

            return View(model);
        }

        [HttpPost]
        public ActionResult Employee(AnnualLeaveBalanceViewModel model)
        {
            model.ExcludedRequestTrans = model.ExcludedRequestTrans ? false : true;
            var getEmployeeCriteria = new GetEmployeeCriteria() { Id = model.EmployeeId };
            ViewBag.Employee = _employeeService.GetEmployeeById(getEmployeeCriteria);
            var getAnnualLeaveTransactionCriteria = new GetAnnualLeaveTransactionCriteria
            {
                EmployeeId = model.EmployeeId,
                Year = model.Year,
                ExcludedRequestTransactions = model.ExcludedRequestTrans
            };

            ViewBag.Data = _employeeService.GetAnnualLeaveTransactions(getAnnualLeaveTransactionCriteria);

            return View(model);
        }

        public ActionResult ExtensionNo()
        {
            return View();
        }

        public ActionResult MyTimeRecord(DateTime? fromDate, DateTime? toDate)
        {
            if (!fromDate.HasValue || !toDate.HasValue)
            {
                fromDate = DateTime.Now.Date;
                toDate = DateTime.Now.Date;
            }

            var timeRecordViewModel = new TimeRecordViewModel
            {
                FromDate = fromDate.Value,
                ToDate = toDate.Value,
                EmployeeId = this.CurrentUser.Id
            };

            return View(timeRecordViewModel);
        }

        public ActionResult TimeRecord(DateTime? fromDate, DateTime? toDate)
        {
            if (!fromDate.HasValue || !toDate.HasValue)
            {
                fromDate = DateTime.Now.Date;
                toDate = DateTime.Now.Date;
            }

            var teamCriteria = new GetAllTeamCriteria();
            IList<Team> teams = _teamService.GetAll(teamCriteria);

            var timeRecordViewModel = new TimeRecordViewModel
            {
                FromDate = fromDate.Value,
                ToDate = toDate.Value,
                Teams = teams
            };

            return View(timeRecordViewModel);
        }

        public ActionResult TimeRecordDetail(DateTime fromDate, DateTime toDate, string groupIds, int? employeeId = null)
        {
            if (!Roles.IsUserInRole("HR"))
            {
                employeeId = this.CurrentUser.Id;
            }

            var criteria = new GetTimeRecordCriteria()
            {
                EmpId = employeeId,
                GroupIds = groupIds,
                FromDate = fromDate,
                ToDate = toDate
            };
            IList<TimeRecordModel> timeRecords = _reportService.GetTimeRecords(criteria);

            return View(timeRecords);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult ProcessInOut(TimeRecordUpdateTypeCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = this.CurrentUser.Id;
                criteria.ModifiedDate = DateTime.Now;
            }

            CommandResult commandResult = _reportService.ProcessInOut(criteria);
            return Json(commandResult, JsonRequestBehavior.AllowGet);
        }
    }
}