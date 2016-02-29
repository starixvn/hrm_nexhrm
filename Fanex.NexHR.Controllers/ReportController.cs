namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.ViewModels.Common;
    using Fanex.NexHR.ViewModels.Report;

    public class ReportController : BaseController
    {
        private readonly IEmployeeService employeeService;
        private readonly IReportService reportService;
        private readonly ITeamService teamService;

        public ReportController(
            IEmployeeService employeeService,
            IReportService reportService,
            ITeamService teamService)
        {
            this.employeeService = employeeService ?? new EmployeeService();
            this.reportService = reportService ?? new ReportService();
            this.teamService = teamService ?? new TeamService();
        }

        public ReportController()
            : this(null, null, null)
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
            model.Teams = teamService.GetAll();

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

            IList<EmployeeAttendanceModel> attendances = reportService.GetEmployeeAttendanceReport(criteria);

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
        public ActionResult ListTimesheet([System.Web.Http.FromUri]TimesheetViewModel model)
        {
            var getEmployeeAttendanceReportCriteria = new GetEmployeeAttendanceReportByTypeCriteria
            {
                FromDate = new DateTime(model.Year, model.Month, 1),
                ToDate = new DateTime(model.Year, model.Month, DateTime.DaysInMonth(model.Year, model.Month)),
                ReportType = model.RequestType,
                EmployeeId = model.EmpId
            };

            IList<EmployeeAttendanceModel> employeeAttendances = reportService.GetEmployeeAttendanceReport(getEmployeeAttendanceReportCriteria);

            return PartialView(employeeAttendances);
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

                    requestMonitorings = reportService.GetRequestMonitoring(getRequestMonitorCriteria);
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

                    requestMonitorings = reportService.GetRequestMonitoring(getRequestMonitorCriteria);
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
            IList<Team> teams = teamService.GetAll();

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

            IList<TotalWorkingHours> totalWorkingHours = reportService.GetTotalWorkingHours(getWorkingHoursCriteria);

            return Json(totalWorkingHours, JsonRequestBehavior.AllowGet);
        }

        #endregion Total Working Hours

        #region Late In/Early Out

        public ActionResult LateInEarlyOut()
        {
            IList<Team> teams = teamService.GetAll();

            var model = new LateInEarlyOutViewModel()
            {
                Year = DateTime.Now.Year,
                Teams = teams
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult ListLateInEarlyOut(LateInEarlyOutViewModel model)
        {
            IList<Team> teams = teamService.GetAll();

            var criteria = new GetLateInEarlyOutReportCriteria
            {
                EmployeeId = model.EmployeeId > 0 ? new int?(model.EmployeeId) : null,
                GroupId = model.TeamId,
                Year = model.Year
            };

            IList<LateInEarlyOutModel> lateInEarlyOuts = reportService.GetLateInEarlyOut(criteria);

            if (model.TeamId.HasValue)
            {
                teams = teams.Where(t => t.Id == model.TeamId.Value).ToList();
            }

            IList<LateInEarlyOutByTeamModel> lateInEarlyOutByTeam = reportService.GetLateInEarlyOutByTeam(lateInEarlyOuts, teams);

            ViewBag.Year = model.Year;

            return PartialView(lateInEarlyOutByTeam);
        }

        [HttpGet]
        public ActionResult LateInEarlyOutDetail(
            DateTime fromDate,
            DateTime toDate,
            int? employeeId = null,
            int? teamId = null)
        {
            IList<Team> teams = teamService.GetAll();

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
                Employee employee = employeeService.GetEmployeeById(getEmployeeCriteria);
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

            IList<LateInEarlyOutDetail> lateInEarlyOuts = reportService.GetLateInEarlyOutDetail(criteria);

            return Json(new { data = lateInEarlyOuts }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LateInEarlyOutDiscipline()
        {
            IList<Team> teams = teamService.GetAll();
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

        [HttpPost]
        public ActionResult ListLateInEarlyOutDiscipline(LateInEarlyOutSummaryViewModel model)
        {
            IList<Team> teams = teamService.GetAll();

            var criteria = new GetLateInEarlyOutSummaryCriteria
            {
                EmployeeId = model.EmployeeId > 0 ? new int?(model.EmployeeId) : null,
                GroupId = model.TeamId,
                FromDate = new DateTime(model.FromYear, model.FromMonth, 1),
                ToDate = new DateTime(model.ToYear, model.ToMonth, DateTime.DaysInMonth(model.ToYear, model.ToMonth)),
                IsUserWarning = model.IsUserWarning
            };

            IList<LateInEarlyOutSummaryModel> lateInEarlyOuts = reportService.GetLateInEarlyOutSummary(criteria);

            if (model.TeamId.HasValue)
            {
                teams = teams.Where(t => t.Id == model.TeamId.Value).ToList();
            }

            IList<LateInEarlyOutSummaryByTeamModel> lateInEarlyOutByTeam = reportService.GetLateInEarlyOutSummaryByTeam(lateInEarlyOuts, teams);

            return PartialView(lateInEarlyOutByTeam);
        }

        #endregion Late In/Early Out

        #region Leave Deduction

        public ActionResult LeaveDeductionDetail(
            DateTime fromDate,
            DateTime toDate,
            int? employeeId = null,
            int? teamId = null)
        {
            IList<Team> teams = teamService.GetAll();

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
                Employee employee = employeeService.GetEmployeeById(getEmployeeCriteria);

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

            IList<LeaveDeductionDetail> leaveDeductions = reportService.GetLeaveDeductionDetail(criteria);

            return Json(new { data = leaveDeductions }, JsonRequestBehavior.AllowGet);
        }

        #endregion Leave Deduction

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

        public ActionResult ListTimeRecord(TimeRecordViewModel model)
        {
            var criteria = new GetTimeRecordCriteria()
            {
                EmpId = this.CurrentUser.Id,
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                PageIndex = model.CurrentPage - 1,
                ItemsPerPage = model.ItemsPerPage
            };

            IList<TimeRecordModel> timeRecords = reportService.GetTimeRecords(criteria);

            var pagerOption = new PagerOptionModel()
            {
                CurrentPage = model.CurrentPage,
                ItemsPerPage = model.ItemsPerPage,
                TotalItem = (int)criteria.TotalItem
            };

            ViewBag.PagerOption = pagerOption;

            return PartialView(timeRecords);
        }

        [Authorize(Roles = "HR")]
        public ActionResult TimeRecord(DateTime? fromDate, DateTime? toDate)
        {
            if (!fromDate.HasValue || !toDate.HasValue)
            {
                fromDate = DateTime.Now.Date;
                toDate = DateTime.Now.Date;
            }

            IList<Team> teams = teamService.GetAll();

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

            IList<TimeRecordModel> timeRecords = reportService.GetTimeRecords(criteria);

            var pagerOption = new PagerOptionModel()
            {
                CurrentPage = model.CurrentPage,
                ItemsPerPage = model.ItemsPerPage,
                TotalItem = (int)criteria.TotalItem
            };

            ViewBag.PagerOption = pagerOption;

            return PartialView("ListTimeRecord", timeRecords);
        }

        #endregion Time Record
    }
}