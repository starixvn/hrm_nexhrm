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
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Overtime;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.ViewModels.AnnualLeave;
    using Fanex.NexHR.ViewModels.Common;
    using Fanex.NexHR.ViewModels.Report;

    [Authorize]
    public class ReportController : BaseController
    {
        private readonly IEmployeeService _employeeService;
        private readonly IReportService _reportService;
        private readonly ITeamService _teamService;
        private readonly IOvertimeService _overtimeService;

        public ReportController(
            IEmployeeService employeeService = null,
            IReportService reportService = null,
            ITeamService teamService = null,
            IOvertimeService overtimeService = null)
        {
            _employeeService = employeeService ?? new EmployeeService();
            _reportService = reportService ?? new ReportService();
            _teamService = teamService ?? new TeamService();
            _overtimeService = overtimeService ?? new OvertimeService();
        }

        public ReportController()
            : this(null, null, null, null)
        {
        }

        #region Attendance

        public ActionResult Attendance(EmployeeAttendanceViewModel model)
        {
            Employee employee = this.CurrentUser;

            if (model.FromDate == DateTime.MinValue)
            {
                model = new EmployeeAttendanceViewModel
                {
                    FromDate = DateTime.Now,
                    ToDate = DateTime.Now,
                    GroupId = employee.Team.Id,
                    DateOption = 0,
                    ExcludeUserDelete = true
                };
            }

            model.CurrentPage = Constants.CurrentPage;
            model.ItemsPerPage = Constants.ItemsPerPage;
            model.Teams = _teamService.GetAll();

            return View(model);
        }

        public ActionResult AttendanceDetail(EmployeeAttendanceViewModel model)
        {
            if (model.GroupId == -1)
            {
                model.GroupId = null;
            }

            var criteria = new GetEmployeeAttendanceReportByTypeCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                ReportType = model.AttendanceType,
                EmployeeId = model.EmpId,
                GroupId = model.GroupId,
                ExcludeUserDelete = model.ExcludeUserDelete ? 1 : 0,
                ItemsPerPage = model.ItemsPerPage,
                PageIndex = model.CurrentPage - 1
            };

            IList<EmployeeAttendanceModel> attendances = _reportService.GetEmployeeAttendanceReport(criteria);

            var pagerOption = new PagerOptionModel()
            {
                CurrentPage = model.CurrentPage,
                ItemsPerPage = model.ItemsPerPage,
                TotalItem = (int)criteria.TotalItem
            };

            ViewBag.PagerOption = pagerOption;

            return PartialView("_AttendanceDetail", attendances);
        }

        #endregion Attendance

        #region Timesheet

        public ActionResult Timesheet()
        {
            var model = new TimesheetViewModel
            {
                Year = DateTime.Now.Year,
                EmpId = CurrentUser.Id
            };

            return View(model);
        }

        [HttpPost]
        public JsonResult Timesheet([System.Web.Http.FromUri]TimesheetViewModel model)
        {
            var getEmployeeAttendanceReportCriteria = new GetEmployeeAttendanceReportByTypeCriteria
            {
                FromDate = new DateTime(model.Year, model.Month, 1),
                ToDate = new DateTime(model.Year, model.Month, DateTime.DaysInMonth(model.Year, model.Month)),
                ReportType = model.RequestType,
                EmployeeId = model.EmpId
            };

            IList<EmployeeAttendanceModel> employeeAttendances = _reportService.GetEmployeeAttendanceReport(getEmployeeAttendanceReportCriteria);

            return Json(new { data = employeeAttendances }, JsonRequestBehavior.AllowGet);
        }

        #endregion Timesheet

        #region HRRequestMonitoring

        [Authorize(Roles = "HR")]
        public ActionResult HRRequestMonitoring()
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;

            var model = new RequestsMonitoringViewModel
            {
                FromDate = new DateTime(year, month, 1),
                ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                MaxRequest = 1,
                DateOption = 0
            };

            ViewBag.ActionName = "HRRequestMonitoringDetail";

            return View("RequestMonitoring", model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult HRRequestMonitoringDetail(RequestsMonitoringViewModel model)
        {
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

            ViewBag.FromDate = model.FromDate;
            ViewBag.ToDate = model.ToDate;
            ViewBag.isLeader = false;
            ViewBag.MaxRequest = model.MaxRequest;

            return PartialView("_RequestMonitoringDetail", requestMonitorings);
        }

        #endregion HRRequestMonitoring

        #region LeaderRequestMonitoring

        [Authorize(Roles = "AssistantLeader")]
        public ActionResult LeaderRequestMonitoring()
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;

            var model = new RequestsMonitoringViewModel
            {
                FromDate = new DateTime(year, month, 1),
                ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                MaxRequest = 1,
                DateOption = 0
            };

            ViewBag.ActionName = "LeaderRequestMonitoringDetail";

            return View("RequestMonitoring", model);
        }

        [Authorize(Roles = "AssistantLeader")]
        public ActionResult LeaderRequestMonitoringDetail(RequestsMonitoringViewModel model)
        {
            IList<RequestsMonitoring> requestMonitorings = new List<RequestsMonitoring>();

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

                    requestMonitorings = _reportService.GetRequestMonitoring(getRequestMonitorCriteria);
                }
            }

            ViewBag.FromDate = model.FromDate;
            ViewBag.ToDate = model.ToDate;
            ViewBag.isLeader = true;
            ViewBag.MaxRequest = model.MaxRequest;

            return PartialView("_RequestMonitoringDetail", requestMonitorings);
        }

        #endregion LeaderRequestMonitoring

        #region Total Working Hours

        public ActionResult TotalWorkingHours()
        {
            IList<Team> teams = _teamService.GetAll();

            var model = new TotalWorkingHoursViewModel
            {
                FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                ToDate = DateTime.Now,
                DateOption = 5,
                IsExportExcel = false,
                Teams = teams
            };

            return View(model);
        }

        [HttpPost]
        public JsonResult TotalWorkingHours(TotalWorkingHoursViewModel model)
        {
            if (model == null)
            {
                int year = DateTime.Now.Year,
                    month = DateTime.Now.Month;

                model = new TotalWorkingHoursViewModel
                {
                    FromDate = new DateTime(year, month, 1),
                    ToDate = DateTime.Now,
                    DateOption = 5,
                    IsExportExcel = false
                };
            }

            var getWorkingHoursCriteria = new GetTotalWorkingHoursSummaryCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                EmployeeId = (model.EmployeeId > 0) ? new int?(model.EmployeeId) : null,
                GroupId = model.TeamId
            };

            IList<TotalWorkingHours> totalWorkingHours = _reportService.GetTotalWorkingHours(getWorkingHoursCriteria);

            return Json(totalWorkingHours, JsonRequestBehavior.AllowGet);
        }

        #endregion Total Working Hours

        #region Leave Summary

        public ActionResult LeaveSummary()
        {
            IList<Team> teams = _teamService.GetAll();

            var model = new LeaveSummaryViewModel
            {
                Year = DateTime.Now.Year,
                EmployeeId = -1,
                Teams = teams
            };

            return View(model);
        }

        public ActionResult LeaveSummaryDetail(LeaveSummaryViewModel model)
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
                teams = _teamService.GetAll();
            }

            ViewBag.CurrentYear = model.Year;
            ViewBag.LastYear = model.Year - 1;
            IList<LeaveSummaryByTeam> leaveSummaryByTeamList = _employeeService.GetAnnualLeaveByTeam(entityList, teams);

            return PartialView("_LeaveSummaryDetail", leaveSummaryByTeamList);
        }

        #endregion Leave Summary

        #region Overtime Request

        public ActionResult MyOvertime()
        {
            var filter = new TimeRecordViewModel
            {
                FromDate = DateTime.Now,
                ToDate = DateTime.Now
            };

            return View(filter);
        }

        [HttpPost]
        public ActionResult MyOvertimeResult(TimeRecordViewModel filter)
        {
            var criteria = new GetMyReportCriteria
            {
                EmployeeId = CurrentUser.Id,
                FromDate = filter.FromDate,
                ToDate = filter.ToDate
            };
            IEnumerable<Overtime> viewModel = _overtimeService.GetMyReport(criteria);

            return View(viewModel);
        }

        public ActionResult OvertimeRequest(OvertimeFilterViewModel model)
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

            return View(model);
        }

        [HttpPost]
        public ActionResult OvertimeRequestDetail(OvertimeFilterViewModel model)
        {
            var criteria = new ReportDetailCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                CreatedBy = model.CreatedBy,
                Status = model.Status,
                EmployeeId = model.EmployeeId
            };

            IEnumerable<Overtime> result = _overtimeService.ReportDetail(criteria);

            return View(result);
        }

        #endregion Overtime Request

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

        #region Late In/Early Out

        public ActionResult LateInEarlyOut()
        {
            IList<Team> teams = _teamService.GetAll();

            var model = new LateInEarlyOutViewModel()
            {
                Year = DateTime.Now.Year,
                Teams = teams
            };

            return View(model);
        }

        [HttpPost]
        public JsonResult LateInEarlyOut(LateInEarlyOutViewModel model)
        {
            IList<Team> teams = _teamService.GetAll();

            var criteria = new GetLateInEarlyOutReportCriteria
            {
                EmployeeId = model.EmployeeId > 0 ? new int?(model.EmployeeId) : null,
                GroupId = model.TeamId,
                Year = model.Year
            };
            IList<LateInEarlyOutModel> lateInEarlyOuts = _reportService.GetLateInEarlyOut(criteria);

            if (model.TeamId.HasValue)
            {
                teams = teams.Where(t => t.Id == model.TeamId.Value).ToList();
            }

            IList<LateInEarlyOutByTeamModel> lateInEarlyOutByTeam = _reportService.GetLateInEarlyOutByTeam(lateInEarlyOuts, teams);

            return Json(new { data = lateInEarlyOutByTeam }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult LateInEarlyOutDetail(
            DateTime fromDate,
            DateTime toDate,
            int? employeeId = null,
            int? teamId = null)
        {
            IList<Team> teams = _teamService.GetAll();

            var model = new LateInEarlyOutDetailViewModel
            {
                EmployeeId = employeeId.HasValue ? employeeId.Value : -1,
                FromDate = fromDate,
                ToDate = toDate,
                IncludeLeaveDeduction = false,
                TeamId = teamId,
                Teams = teams
            };

            if (employeeId.HasValue)
            {
                var getEmployeeCriteria = new GetEmployeeByIdCriteria() { EmployeeId = employeeId.Value };
                Employee employee = _employeeService.GetEmployeeById(getEmployeeCriteria);
                model.Employee = employee.DisplayName;
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult LateInEarlyOutDetail(LateInEarlyOutDetailViewModel model)
        {
            var criteria = new GetLateInEarlyOutDetailReportCriteria()
            {
                EmployeeId = model.EmployeeId <= 0 ? null : new int?(model.EmployeeId),
                GroupId = model.TeamId,
                IncludeLeaveDeduction = model.IncludeLeaveDeduction,
                FromDate = model.FromDate,
                ToDate = model.ToDate
            };

            IList<LateInEarlyOutDetail> lateInEarlyOuts = _reportService.GetLateInEarlyOutDetail(criteria);

            return Json(new { data = lateInEarlyOuts }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LateInEarlyOutSummary()
        {
            IList<Team> teams = _teamService.GetAll();
            var today = DateTime.Today;

            var model = new LateInEarlyOutSummaryViewModel()
            {
                FromYear = today.AddYears(-1).Year,
                FromMonth = today.AddMonths(1).Month,
                ToYear = today.Year,
                ToMonth = today.Month,
                Teams = teams
            };

            return View(model);
        }

        #endregion Late In/Early Out

        #region Leave Deduction

        public ActionResult LeaveDeductionDetail(
            DateTime fromDate,
            DateTime toDate,
            int? employeeId = null,
            int? teamId = null)
        {
            IList<Team> teams = _teamService.GetAll();

            var model = new LeaveDeductionDetailViewModel
            {
                EmployeeId = employeeId.HasValue ? employeeId.Value : -1,
                FromDate = fromDate,
                ToDate = toDate,
                TeamId = teamId,
                Teams = teams
            };

            if (employeeId.HasValue)
            {
                var getEmployeeCriteria = new GetEmployeeByIdCriteria() { EmployeeId = employeeId.Value };
                Employee employee = _employeeService.GetEmployeeById(getEmployeeCriteria);

                model.Employee = employee.DisplayName;
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult LeaveDeductionDetail(LeaveDeductionDetailViewModel model)
        {
            var criteria = new GetLeaveDeductionDetailCriteria()
            {
                EmployeeId = model.EmployeeId <= 0 ? null : new int?(model.EmployeeId),
                GroupId = model.TeamId,
                FromDate = model.FromDate,
                ToDate = model.ToDate
            };

            IList<LeaveDeductionDetail> leaveDeductions = _reportService.GetLeaveDeductionDetail(criteria);

            return Json(new { data = leaveDeductions }, JsonRequestBehavior.AllowGet);
        }

        #endregion Leave Deduction

        [HttpPost]
        public ActionResult Employee(AnnualLeaveBalanceViewModel model)
        {
            model.ExcludedRequestTrans = model.ExcludedRequestTrans ? false : true;
            var getEmployeeCriteria = new GetEmployeeByIdCriteria() { EmployeeId = model.EmployeeId };
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

        #region Time Record

        public ActionResult MyTimeRecord(DateTime? fromDate, DateTime? toDate)
        {
            if (!fromDate.HasValue || !toDate.HasValue)
            {
                fromDate = DateTime.Now.Date;
                toDate = DateTime.Now.Date;
            }

            var model = new TimeRecordViewModel
            {
                FromDate = fromDate.Value,
                ToDate = toDate.Value,
                EmployeeId = this.CurrentUser.Id,
                CurrentPage = Constants.CurrentPage,
                ItemsPerPage = Constants.ItemsPerPage
            };

            return View(model);
        }

        public ActionResult MyTimeRecordDetail(TimeRecordViewModel model)
        {
            var criteria = new GetTimeRecordCriteria()
            {
                EmpId = this.CurrentUser.Id,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                PageIndex = model.CurrentPage - 1,
                ItemsPerPage = model.ItemsPerPage
            };

            IList<TimeRecordModel> timeRecords = _reportService.GetTimeRecords(criteria);

            var pagerOption = new PagerOptionModel()
            {
                CurrentPage = model.CurrentPage,
                ItemsPerPage = model.ItemsPerPage,
                TotalItem = (int)criteria.TotalItem
            };

            ViewBag.PagerOption = pagerOption;

            return PartialView("_TimeRecordDetail", timeRecords);
        }

        [Authorize(Roles = "HR")]
        public ActionResult TimeRecord(DateTime? fromDate, DateTime? toDate)
        {
            if (!fromDate.HasValue || !toDate.HasValue)
            {
                fromDate = DateTime.Now.Date;
                toDate = DateTime.Now.Date;
            }

            IList<Team> teams = _teamService.GetAll();

            var model = new TimeRecordViewModel
            {
                FromDate = fromDate.Value,
                ToDate = toDate.Value,
                Teams = teams,
                CurrentPage = Constants.CurrentPage,
                ItemsPerPage = Constants.ItemsPerPage
            };

            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult TimeRecordDetail(TimeRecordViewModel model)
        {
            int? employeeId = null;

            var criteria = new GetTimeRecordCriteria()
            {
                EmpId = model.EmployeeId == 0 ? employeeId : model.EmployeeId,
                GroupIds = model.TeamIds,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                PageIndex = model.CurrentPage - 1,
                ItemsPerPage = model.ItemsPerPage
            };

            IList<TimeRecordModel> timeRecords = _reportService.GetTimeRecords(criteria);

            var pagerOption = new PagerOptionModel()
            {
                CurrentPage = model.CurrentPage,
                ItemsPerPage = model.ItemsPerPage,
                TotalItem = (int)criteria.TotalItem
            };

            ViewBag.PagerOption = pagerOption;

            return PartialView("_TimeRecordDetail", timeRecords);
        }

        #endregion Time Record

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