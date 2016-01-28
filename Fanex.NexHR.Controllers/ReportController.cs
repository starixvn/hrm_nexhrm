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
    using Fanex.NexHR.ViewModels.Common;
    using Fanex.NexHR.ViewModels.Report;

    [Authorize]
    public class ReportController : BaseController
    {
        private const int CurrentPage = 1;
        private const int ItemsPerPage = 50;

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
            Employee employee = this.CurrentUser;

            if (model.FromDate == DateTime.MinValue)
            {
                model = new EmployeeAttendanceViewModel
                {
                    FromDate = DateTime.Now,
                    ToDate = DateTime.Now,
                    GroupId = employee.Team.Id,
                    DateOption = 0,
                    ExcludeUserDelete = true,
                    ItemsPerPageArray = new PagerOptionModel().ItemsPerPageArray
                };
            }

            model.TeamCollection = _teamService.GetAll(new GetAllTeamCriteria());

            return View(model);
        }

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
                EmployeeId = model.EmpId,
                GetAll = true
            };

            DetailEmployeeAttendanceModel detailAttendance = _reportService.GetEmployeeAttendanceReport(getEmployeeAttendanceReportCriteria);
            var data = detailAttendance.EmployeeAttendances;

            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
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
            IList<Team> teams = _teamService.GetAll(new GetAllTeamCriteria()); ;

            var model = new TotalWorkingHoursViewModel
            {
                FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                ToDate = DateTime.Now,
                DateOption = 5,
                IsExportExcel = false,
                TeamCollection = teams
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
            IList<Team> teams = _teamService.GetAll(new GetAllTeamCriteria());

            var model = new LeaveSummaryViewModel
            {
                Year = DateTime.Now.Year,
                EmployeeId = -1,
                TeamCollection = teams
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
                teams = _teamService.GetAll(new GetAllTeamCriteria());
            }

            ViewBag.CurrentYear = model.Year;
            ViewBag.LastYear = model.Year - 1;
            IList<LeaveSummaryByTeam> leaveSummaryByTeamList = _employeeService.GetAnnualLeaveByTeam(entityList, teams);

            return PartialView("_LeaveSummaryDetail", leaveSummaryByTeamList);
        }

        #endregion Leave Summary

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

        public ActionResult LateInEarlyOutSummary()
        {
            IList<Team> teams = _teamService.GetAll(new GetAllTeamCriteria());

            var model = new LateInEarlyOutSummaryViewModel()
            {
                Year = DateTime.Now.Year,
                TeamCollection = teams
            };

            return View(model);
        }

        [HttpPost]
        public JsonResult LateInEarlyOutSummary(LateInEarlyOutSummaryViewModel model)
        {
            IList<Team> teams = _teamService.GetAll(new GetAllTeamCriteria());

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

            return Json(new { data = lateInEarlyOutSummaryByTeam }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult LateInEarlyOutDetail(
            DateTime fromDate,
            DateTime toDate,
            int? employeeId = null,
            int? teamId = null)
        {
            IList<Team> teams = _teamService.GetAll(new GetAllTeamCriteria());

            var model = new LateInEarlyOutDetailViewModel
            {
                EmployeeId = employeeId.HasValue ? employeeId.Value : -1,
                FromDate = fromDate,
                ToDate = toDate,
                IncludeLeaveDeduction = false,
                TeamId = teamId,
                TeamCollection = teams
            };

            if (employeeId.HasValue)
            {
                var getEmployeeCriteria = new GetEmployeeCriteria() { Id = employeeId.Value };
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

        #endregion Late In/Early Out

        #region Leave Deduction

        public ActionResult LeaveDeductionDetail(
            DateTime fromDate,
            DateTime toDate,
            int? employeeId = null,
            int? teamId = null)
        {
            IList<Team> teams = _teamService.GetAll(new GetAllTeamCriteria());

            var model = new LeaveDeductionDetailViewModel
            {
                EmployeeId = employeeId.HasValue ? employeeId.Value : -1,
                FromDate = fromDate,
                ToDate = toDate,
                TeamId = teamId,
                TeamCollection = teams
            };

            if (employeeId.HasValue)
            {
                var getEmployeeCriteria = new GetEmployeeCriteria() { Id = employeeId.Value };
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
                CurrentPage = CurrentPage,
                ItemsPerPage = ItemsPerPage
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
            int totalPage = 0;

            if (criteria.TotalItem != null)
            {
                totalPage = (int)criteria.TotalItem / model.ItemsPerPage;
                totalPage = decimal.Divide((int)criteria.TotalItem, model.ItemsPerPage) == 0 ? totalPage : (totalPage + 1);
            }

            var pagerOption = new PagerOptionModel()
            {
                CurrentPage = model.CurrentPage,
                ItemsPerPage = model.ItemsPerPage,
                TotalItem = (int)criteria.TotalItem,
                TotalPage = totalPage
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

            IList<Team> teams = _teamService.GetAll(new GetAllTeamCriteria());

            var model = new TimeRecordViewModel
            {
                FromDate = fromDate.Value,
                ToDate = toDate.Value,
                Teams = teams,
                CurrentPage = CurrentPage,
                ItemsPerPage = ItemsPerPage
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
            int totalPage = 0;

            if (criteria.TotalItem != null)
            {
                totalPage = (int)criteria.TotalItem / model.ItemsPerPage;
                totalPage = decimal.Divide((int)criteria.TotalItem, model.ItemsPerPage) == 0 ? totalPage : (totalPage + 1);
            }

            var pagerOption = new PagerOptionModel()
            {
                CurrentPage = model.CurrentPage,
                ItemsPerPage = model.ItemsPerPage,
                TotalItem = (int)criteria.TotalItem,
                TotalPage = totalPage
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