namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
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
    using Fanex.NexHR.ViewModels.TotalWorkingHours;

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

        public ActionResult Attendance(EmployeeAttendanceViewModel model)
        {
            if (model != null)
            {
                if (model.FromDate == DateTime.MinValue)
                {
                    model = new EmployeeAttendanceViewModel
                    {
                        FromDate = DateTime.Now,
                        ToDate = DateTime.Now,
                        DateOption = 0,
                        ExcludeUserDelete = true
                    };
                }

                model.CurrentPage = Constants.CurrentPage;
                model.ItemsPerPage = 200;
                model.Teams = teamService.GetTeam();
            }

            return View(model);
        }

        [ValidateInput(false)]
        public ActionResult ListAttendance(EmployeeAttendanceViewModel model)
        {
            IList<EmployeeAttendanceModel> attendances = new List<EmployeeAttendanceModel>();
            if (model != null)
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
                    EmployeeId = model.EmployeeId,
                    GroupId = model.GroupId,
                    ExcludeUserDelete = model.ExcludeUserDelete,
                    ItemsPerPage = model.ItemsPerPage,
                    PageIndex = model.CurrentPage - 1
                };

                attendances = reportService.GetEmployeeAttendanceReport(criteria);
                var pagerOption = new PagerOptionModel()
                {
                    CurrentPage = model.CurrentPage,
                    ItemsPerPage = model.ItemsPerPage,
                    TotalItem = (int)criteria.TotalItem
                };
                ViewBag.PagerOption = pagerOption;
            }

            return PartialView("_ListAttendance", attendances);
        }

        [Authorize(Roles = "SpecialUser")]
        public JsonResult GetEmployeeAttendance(EmployeeAttendanceViewModel model)
        {
            IList<EmployeeAttendanceModel> employeeAttendances = new List<EmployeeAttendanceModel>();
            if (model != null)
            {
                if (model.GroupId == -1)
                {
                    model.GroupId = null;
                }

                var getEmployeeAttendanceReportCriteria = new GetEmployeeAttendanceReportByTypeCriteria
                {
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    ReportType = model.AttendanceType,
                    EmployeeId = model.EmployeeId,
                    GroupId = model.GroupId,
                    ExcludeUserDelete = model.ExcludeUserDelete
                };
                employeeAttendances = reportService.GetEmployeeAttendanceReport(getEmployeeAttendanceReportCriteria);
            }

            return Json(employeeAttendances);
        }

        [Authorize(Roles = "SpecialUser")]
        public ActionResult Timesheet()
        {
            var model = new TimesheetViewModel
            {
                Year = DateTime.Now.Year,
                EmpId = CurrentUser.Id
            };

            return View(model);
        }

        [Authorize(Roles = "SpecialUser")]
        [HttpPost]
        public ActionResult ListTimesheet(TimesheetViewModel model)
        {
            IList<EmployeeAttendanceModel> employeeAttendances = new List<EmployeeAttendanceModel>();
            if (model != null)
            {
                var getEmployeeAttendanceReportCriteria = new GetEmployeeAttendanceReportByTypeCriteria
                {
                    FromDate = new DateTime(model.Year, model.Month, 1),
                    ToDate = new DateTime(model.Year, model.Month, DateTime.DaysInMonth(model.Year, model.Month)),
                    ReportType = model.RequestType,
                    EmployeeId = model.EmpId
                };

                employeeAttendances = reportService.GetEmployeeAttendanceReport(getEmployeeAttendanceReportCriteria);
            }

            return PartialView("_ListTimesheet", employeeAttendances);
        }

        [Authorize(Roles = "HR")]
        public ActionResult HRRequestMonitoring()
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;

            var model = new RequestsMonitoringViewModel
            {
                FromDate = new DateTime(year, month, 1),
                ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                MaxRequest = 2,
                DateOption = 0,
                IsLeader = false
            };

            return View("RequestMonitoring", model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult ListHRRequestMonitoring(RequestsMonitoringViewModel model)
        {
            IList<RequestsMonitoring> requestMonitorings = GetRequestMonitoringDetail(model);
            return PartialView("_ListRequestMonitoring", requestMonitorings);
        }

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
                DateOption = 0,
                IsLeader = true
            };

            return View("RequestMonitoring", model);
        }

        [Authorize(Roles = "AssistantLeader")]
        public ActionResult ListLeaderRequestMonitoring(RequestsMonitoringViewModel model)
        {
            IList<RequestsMonitoring> requestMonitorings = GetRequestMonitoringDetail(model);
            return PartialView("_ListRequestMonitoring", requestMonitorings);
        }

        private IList<RequestsMonitoring> GetRequestMonitoringDetail(RequestsMonitoringViewModel model)
        {
            var criteria = new GetRequestMonitorReportCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                MaxRequest = model.MaxRequest,
                Role = model.IsLeader ? (int)Role.AssistantLeader : (int)Role.HR,
                EmployeeId = CurrentUser.Id
            };

            IList<RequestsMonitoring> requestMonitorings = reportService.GetRequestMonitoring(criteria);

            ViewBag.FromDate = model.FromDate;
            ViewBag.ToDate = model.ToDate;
            ViewBag.isLeader = model.IsLeader;
            ViewBag.MaxRequest = model.MaxRequest;

            return requestMonitorings;
        }

        public ActionResult TotalWorkingHour()
        {
            IList<Team> teams = teamService.GetTeam();

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

        [ValidateInput(false)]
        [HttpPost]
        public JsonResult GetTotalWorkingHour(TotalWorkingHoursViewModel model)
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

        [ValidateInput(false)]
        public ActionResult LateInEarlyOut(int empId = 0, string nickname = "")
        {
            IList<Team> teams = teamService.GetTeam();

            LateInEarlyOutViewModel model;
            if (empId != 0)
            {
                model = new LateInEarlyOutViewModel()
                {
                    Year = DateTime.Now.Year,
                    Teams = teams,
                    EmployeeId = empId,
                    Employee = nickname
                };
            }
            else
            {
                model = new LateInEarlyOutViewModel()
                {
                    Year = DateTime.Now.Year,
                    Teams = teams,
                };
            }

            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult ListLateInEarlyOut(LateInEarlyOutViewModel model)
        {
            IList<LateInEarlyOutModel> lateInEarlyOuts = new List<LateInEarlyOutModel>();
            if (model != null)
            {
                var criteria = new GetLateInEarlyOutReportCriteria
                {
                    EmployeeId = model.EmployeeId > 0 ? new int?(model.EmployeeId) : null,
                    GroupId = model.TeamId,
                    Year = model.Year
                };

                lateInEarlyOuts = reportService.GetLateInEarlyOut(criteria);

                ViewBag.Year = model.Year;
            }

            return PartialView("_ListLateInEarlyOut", lateInEarlyOuts);
        }

        [HttpGet]
        public ActionResult LateInEarlyOutDetail(
            DateTime fromDate,
            DateTime toDate,
            int? employeeId = null,
            int? teamId = null)
        {
            IList<Team> teams = teamService.GetTeam();

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
        public JsonResult GetLateInEarlyOutDetail(LateInEarlyOutDetailViewModel model)
        {
            IList<LateInEarlyOutDetail> lateInEarlyOuts = new List<LateInEarlyOutDetail>();
            if (model != null)
            {
                var criteria = new GetLateInEarlyOutDetailReportCriteria()
                {
                    EmployeeId = model.EmployeeId <= 0 ? null : new int?(model.EmployeeId),
                    GroupId = model.TeamId,
                    IncludeLeaveDeduction = model.IncludeLeaveDeduction,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate
                };

                lateInEarlyOuts = reportService.GetLateInEarlyOutDetail(criteria);
            }

            return Json(lateInEarlyOuts);
        }

        [Authorize(Roles = "HR")]
        public ActionResult LateInEarlyOutDiscipline()
        {
            IList<Team> teams = teamService.GetTeam();
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

        [ValidateInput(false)]
        [Authorize(Roles = "HR")]
        [HttpPost]
        public ActionResult ListLateInEarlyOutDiscipline(LateInEarlyOutSummaryViewModel model)
        {
            IList<LateInEarlyOutDiscipline> lateInEarlyOutDisciplines = new List<LateInEarlyOutDiscipline>();
            if (model != null)
            {
                var toDate = new DateTime(model.ToYear, model.ToMonth, DateTime.DaysInMonth(model.ToYear, model.ToMonth));
                var criteria = new GetLateInEarlyOutSummaryCriteria
                {
                    EmployeeId = model.EmployeeId > 0 ? new int?(model.EmployeeId) : null,
                    GroupId = model.TeamId,
                    FromDate = toDate.AddYears(-1).AddDays(1),
                    ToDate = new DateTime(model.ToYear, model.ToMonth, DateTime.DaysInMonth(model.ToYear, model.ToMonth)),
                    IsUserWarning = true
                };

                lateInEarlyOutDisciplines = reportService.GetLateInEarlyOutDiscipline(criteria);
            }

            return PartialView("_ListLateInEarlyOutDiscipline", lateInEarlyOutDisciplines);
        }

        [Authorize(Roles = "SpecialUser")]
        public ActionResult MyTimeRecord(DateTime? fromDate, DateTime? toDate)
        {
            if (!fromDate.HasValue || !toDate.HasValue)
            {
                fromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                toDate = DateTime.Today;
            }

            var model = new TimeRecordViewModel
            {
                FromDate = fromDate.Value,
                ToDate = toDate.Value,
                EmployeeId = CurrentUser.Id,
                CurrentPage = Constants.CurrentPage,
                ItemsPerPage = Constants.ItemsPerPage
            };

            return View(model);
        }

        [Authorize(Roles = "SpecialUser")]
        public ActionResult ListMyTimeRecord(TimeRecordViewModel model)
        {
            IList<TimeRecord> timeRecords = new List<TimeRecord>();
            if (model != null)
            {
                var criteria = new GetTimeRecordCriteria()
                {
                    EmpId = CurrentUser.Id,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    PageIndex = model.CurrentPage - 1,
                    ItemsPerPage = model.ItemsPerPage
                };
                timeRecords = reportService.GetTimeRecords(criteria);

                var pagerOption = new PagerOptionModel()
                {
                    CurrentPage = model.CurrentPage,
                    ItemsPerPage = model.ItemsPerPage,
                    TotalItem = (int)criteria.TotalItem
                };

                ViewBag.PagerOption = pagerOption;
            }

            return PartialView("_ListTimeRecord", timeRecords);
        }

        [Authorize(Roles = "HR")]
        public ActionResult TimeRecord(DateTime? fromDate, DateTime? toDate)
        {
            if (!fromDate.HasValue || !toDate.HasValue)
            {
                fromDate = DateTime.Now.Date;
                toDate = DateTime.Now.Date;
            }

            IList<Team> teams = teamService.GetTeam();

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

        [ValidateInput(false)]
        [Authorize(Roles = "HR")]
        public ActionResult ListTimeRecord(TimeRecordViewModel model)
        {
            IList<TimeRecord> timeRecords = new List<TimeRecord>();
            if (model != null)
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

                timeRecords = reportService.GetTimeRecords(criteria);

                var pagerOption = new PagerOptionModel()
                {
                    CurrentPage = model.CurrentPage,
                    ItemsPerPage = model.ItemsPerPage,
                    TotalItem = (int)criteria.TotalItem
                };

                ViewBag.PagerOption = pagerOption;
            }

            return PartialView("_ListTimeRecord", timeRecords);
        }
    }
}