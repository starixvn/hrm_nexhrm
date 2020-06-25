namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Core;
    using Core.Enums;
    using Criteria.Employee;
    using Criteria.Report;
    using Fanex.NexHR.Core.Model;
    using Models;
    using Models.Report;
    using Newtonsoft.Json;
    using Services;
    using Services.Interfaces;
    using ViewModels;
    using ViewModels.Common;
    using ViewModels.Report;
    using ViewModels.TotalWorkingHours;

    public class ReportController : BaseController
    {
        private readonly IEmployeeService _employeeService;
        private readonly IReportService _reportService;
        private readonly ITeamService _teamService;

        public ReportController(
            IEmployeeService employeeService,
            IReportService reportService,
            ITeamService teamService)
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
            const int numberItems = 200;
            var employeeAttendanceViewModel = model;
            if (employeeAttendanceViewModel != null)
            {
                if (employeeAttendanceViewModel.FromDate == DateTime.MinValue)
                {
                    employeeAttendanceViewModel = new EmployeeAttendanceViewModel
                    {
                        FromDate = DateTime.Now,
                        ToDate = DateTime.Now,
                        DateOption = 0,
                        ExcludeUserDelete = true
                    };
                }

                employeeAttendanceViewModel.CurrentPage = Constants.CurrentPage;
                employeeAttendanceViewModel.ItemsPerPage = numberItems;
                employeeAttendanceViewModel.Teams = _teamService.GetTeam();
            }

            return View(employeeAttendanceViewModel);
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

                attendances = _reportService.GetEmployeeAttendanceReport(criteria);
                if (criteria.TotalItem != null)
                {
                    var pagerOption = new PagerOptionModel
                    {
                        CurrentPage = model.CurrentPage,
                        ItemsPerPage = model.ItemsPerPage,
                        TotalItem = (int)criteria.TotalItem
                    };
                    ViewBag.PagerOption = pagerOption;
                }

                attendances = SortAttendance(model.Sort, attendances);
            }
            return PartialView("_ListAttendance", attendances);
        }

        private static IList<EmployeeAttendanceModel> SortAttendance(string sort, IList<EmployeeAttendanceModel> attendances)
        {
            IList<EmployeeAttendanceModel> attendancesSorted;

            if (sort != null)
            {
                var sorts = JsonConvert.DeserializeObject<List<SortModel>>(sort);
                attendancesSorted = Utilities.MultipleSort(attendances, sorts);
            }
            else
            {
                attendancesSorted = attendances;
            }

            return attendancesSorted;
        }

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
                employeeAttendances = _reportService.GetEmployeeAttendanceReport(getEmployeeAttendanceReportCriteria);
            }

            return Json(employeeAttendances);
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

                employeeAttendances = _reportService.GetEmployeeAttendanceReport(getEmployeeAttendanceReportCriteria);
            }

            return PartialView("_ListTimesheet", employeeAttendances);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Hr", Justification = "Reviewed")]
        [Authorize(Roles = "HR")]
        public ActionResult HrRequestMonitoring()
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;

            var model = new RequestsMonitoringViewModel
            {
                FromDate = new DateTime(year, month, 1),
                ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                RequestType = 0,
                MaxRequest = 2,
                DateOption = 0,
                IsLeader = false
            };

            return View("RequestMonitoring", model);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Hr", Justification = "Reviewed")]
        [Authorize(Roles = "HR")]
        public ActionResult ListHrRequestMonitoring(RequestsMonitoringViewModel model)
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
                RequestType = 0,
                MaxRequest = 1,
                DateOption = 0,
                IsLeader = true
            };

            return View("RequestMonitoring", model);
        }

        [Authorize(Roles = "AssistantLeader")]
        public ActionResult ListLeaderRequestMonitoring(RequestsMonitoringViewModel model)
        {
            return PartialView("_ListRequestMonitoring", GetRequestMonitoringDetail(model));
        }

        private IList<RequestsMonitoring> GetRequestMonitoringDetail(RequestsMonitoringViewModel model)
        {
            var criteria = new GetRequestMonitorReportCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                RequestType = model.RequestType,
                MaxRequest = model.MaxRequest,
                Role = model.IsLeader ? (int)Role.AssistantLeader : (int)Role.HR,
                EmployeeId = CurrentUser.Id
            };

            IList<RequestsMonitoring> requestMonitorings = _reportService.GetRequestMonitoring(criteria);

            ViewBag.FromDate = model.FromDate;
            ViewBag.ToDate = model.ToDate;
            ViewBag.isLeader = model.IsLeader;
            ViewBag.MaxRequest = model.MaxRequest;

            return requestMonitorings;
        }

        public ActionResult TotalWorkingHour()
        {
            IList<Team> teams = _teamService.GetTeam();

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
            const int dayOptions = 5;
            var totalWorkingHoursViewModel = model;
            if (totalWorkingHoursViewModel == null)
            {
                int year = DateTime.Now.Year;
                int month = DateTime.Now.Month;

                totalWorkingHoursViewModel = new TotalWorkingHoursViewModel
                {
                    FromDate = new DateTime(year, month, 1),
                    ToDate = DateTime.Now,
                    DateOption = dayOptions,
                    IsExportExcel = false
                };
            }

            var getWorkingHoursCriteria = new GetTotalWorkingHoursSummaryCriteria
            {
                FromDate = totalWorkingHoursViewModel.FromDate,
                ToDate = totalWorkingHoursViewModel.ToDate,
                EmployeeId = (totalWorkingHoursViewModel.EmployeeId > 0) ? new int?(totalWorkingHoursViewModel.EmployeeId) : null,
                GroupId = totalWorkingHoursViewModel.TeamId
            };

            IList<TotalWorkingHours> totalWorkingHours = _reportService.GetTotalWorkingHours(getWorkingHoursCriteria);

            IList<TotalWorkingHours> totalWorkingHour = SortTotalWorking(model?.Sort, totalWorkingHours);

            return Json(totalWorkingHour, JsonRequestBehavior.AllowGet);
        }

        private static IList<TotalWorkingHours> SortTotalWorking(string sort, IList<TotalWorkingHours> totalWorkingHours)
        {
            IList<TotalWorkingHours> totalWorkingSorted;

            if (sort != null)
            {
                var sorts = JsonConvert.DeserializeObject<List<SortModel>>(sort);
                totalWorkingSorted = Utilities.MultipleSort(totalWorkingHours, sorts);
            }
            else
            {
                totalWorkingSorted = totalWorkingHours;
            }

            return totalWorkingSorted;
        }

        [ValidateInput(false)]
        public ActionResult LateInEarlyOut(int empId = 0, string nickname = "")
        {
            IList<Team> teams = _teamService.GetTeam();

            var model = empId != 0
                ? new LateInEarlyOutViewModel
                {
                    Year = DateTime.Now.Year,
                    Teams = teams,
                    EmployeeId = empId,
                    Employee = nickname
                }
                : new LateInEarlyOutViewModel
                {
                    Year = DateTime.Now.Year,
                    Teams = teams,
                };

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

                lateInEarlyOuts = _reportService.GetLateInEarlyOut(criteria);

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
            IList<Team> teams = _teamService.GetTeam();

            var model = new LateInEarlyOutDetailViewModel
            {
                EmployeeId = employeeId ?? -1,
                FromDate = fromDate,
                ToDate = toDate,
                IncludeLeaveDeduction = false,
                TeamId = teamId,
                Teams = teams
            };

            if (employeeId.HasValue)
            {
                var getEmployeeCriteria = new GetEmployeeByIdCriteria { EmployeeId = employeeId.Value };
                Employee employee = _employeeService.GetEmployeeById(getEmployeeCriteria);
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
                var criteria = new GetLateInEarlyOutDetailReportCriteria
                {
                    EmployeeId = model.EmployeeId <= 0 ? null : new int?(model.EmployeeId),
                    GroupId = model.TeamId,
                    IncludeLeaveDeduction = model.IncludeLeaveDeduction,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate
                };

                lateInEarlyOuts = _reportService.GetLateInEarlyOutDetail(criteria);
            }

            return Json(lateInEarlyOuts);
        }

        [Authorize(Roles = "HR")]
        public ActionResult LateInEarlyOutDiscipline()
        {
            IList<Team> teams = _teamService.GetTeam();
            var today = DateTime.Today;

            var model = new LateInEarlyOutSummaryViewModel
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

                lateInEarlyOutDisciplines = _reportService.GetLateInEarlyOutDiscipline(criteria);
            }

            return PartialView("_ListLateInEarlyOutDiscipline", lateInEarlyOutDisciplines);
        }

        public ActionResult MyTimeRecord(DateTime? fromDate, DateTime? toDate)
        {
            DateTime from;
            DateTime to;
            if (!fromDate.HasValue || !toDate.HasValue)
            {
                from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                to = DateTime.Today;
            }
            else
            {
                from = fromDate.Value;
                to = toDate.Value;
            }

            var model = new TimeRecordViewModel
            {
                FromDate = from,
                ToDate = to,
                EmployeeId = CurrentUser.Id,
                CurrentPage = Constants.CurrentPage,
                ItemsPerPage = Constants.ItemsPerPage
            };

            return View(model);
        }

        public ActionResult ListMyTimeRecord(TimeRecordViewModel model)
        {
            IList<TimeRecord> timeRecords = new List<TimeRecord>();
            if (model != null)
            {
                var criteria = new GetTimeRecordCriteria
                {
                    EmpId = CurrentUser.Id,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    PageIndex = model.CurrentPage - 1,
                    ItemsPerPage = model.ItemsPerPage
                };
                timeRecords = _reportService.GetTimeRecords(criteria);

                if (criteria.TotalItem != null)
                {
                    var pagerOption = new PagerOptionModel
                    {
                        CurrentPage = model.CurrentPage,
                        ItemsPerPage = model.ItemsPerPage,
                        TotalItem = (int)criteria.TotalItem
                    };

                    ViewBag.PagerOption = pagerOption;
                }
            }

            return PartialView("_ListTimeRecord", timeRecords);
        }

        [Authorize(Roles = "HR")]
        public ActionResult TimeRecord(DateTime? fromDate, DateTime? toDate)
        {
            DateTime from;
            DateTime to;

            if (!fromDate.HasValue || !toDate.HasValue)
            {
                from = DateTime.Now.Date;
                to = DateTime.Now.Date;
            }
            else
            {
                from = fromDate.Value;
                to = toDate.Value;
            }

            IList<Team> teams = _teamService.GetTeam();

            var model = new TimeRecordViewModel
            {
                FromDate = from,
                ToDate = to,
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
                var criteria = new GetTimeRecordCriteria
                {
                    EmpId = model.EmployeeId,
                    GroupIds = model.TeamIds,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    PageIndex = model.CurrentPage - 1,
                    ItemsPerPage = model.ItemsPerPage
                };

                timeRecords = _reportService.GetTimeRecords(criteria);

                if (criteria.TotalItem != null)
                {
                    var pagerOption = new PagerOptionModel
                    {
                        CurrentPage = model.CurrentPage,
                        ItemsPerPage = model.ItemsPerPage,
                        TotalItem = (int)criteria.TotalItem
                    };

                    ViewBag.PagerOption = pagerOption;
                }
            }

            return PartialView("_ListTimeRecord", timeRecords);
        }

        [ValidateInput(false)]
        [HttpPost]
        public JsonResult GetWorkingHoursStatistic(WorkingHoursStatisticViewModel model)
        {
            var getWorkingHoursCriteria = new GetWorkingHoursStatisticCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                FromDateLastMonth = model.FromDateLastMonth,
                ToDateLastMonth = model.ToDateLastMonth,
                EmployeeId = (model.EmployeeId > 0) ? new int?(model.EmployeeId) : null,
                GroupId = model.TeamId
            };

            IList<WorkingHoursStatistic> totalWorkingHours = _reportService.GetWorkingHoursStatistic(getWorkingHoursCriteria);

            return Json(totalWorkingHours, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "HR")]
        public ActionResult WorkingHoursStatistic()
        {
            IList<Team> teams = _teamService.GetTeam();

            DateTime date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var model = new WorkingHoursStatisticViewModel
            {
                FromDate = date,
                ToDate = date.AddMonths(1).AddDays(-1),
                FromDateLastMonth = date.AddMonths(-1).AddDays(25),
                ToDateLastMonth = date.AddDays(-1),
                DateOption = 0,
                IsExportExcel = false,
                Teams = teams,
                Month = date.Month,
                Year = date.Year
            };
            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult HeadCount()
        {
            return View();
        }

        [Authorize(Roles = "HR")]
        public ActionResult GetHeadCount(int year)
        {
            var critera = new GetHeadCountByYearCriteria
            {
                StartDateOfYear = new DateTime(year, 1, 1),
                EndDateOfYear = new DateTime(year, 12, 31)
            };

            var model = new HeadCountModel
            {
                StartDate = critera.StartDateOfYear,
                EndDate = critera.EndDateOfYear,
                HeadCount = _reportService.GetTeamHeadCountInYear(critera)
            };
            return PartialView("_HeadCountTable", model);
        }
    }
}