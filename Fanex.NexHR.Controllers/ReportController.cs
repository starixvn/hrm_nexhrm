namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using Core;
    using Core.Enums;
    using Criteria.Employee;
    using Criteria.Report;
    using Fanex.NexHR.Controllers.Attributes;
    using Fanex.NexHR.Core.Model;
    using Fanex.NexHR.ViewModels.YearlyNexRequest;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Models;
    using Models.Report;
    using Newtonsoft.Json;
    using Services.Interfaces;
    using ViewModels;
    using ViewModels.Common;
    using ViewModels.Report;
    using ViewModels.TotalWorkingHours;

    public class ReportController : BaseController
    {
        private readonly IEmployeeService employeeService;
        private readonly IReportService reportService;
        private readonly ITeamService teamService;

        private readonly IDepartmentService departmentService;
        private readonly IBaseService baseService;

        private readonly bool isEditableSettingCheckInOut;

        public ReportController(ISessionService sessionService,
            IEmployeeService employeeService,
            IReportService reportService,
            ITeamService teamService,
            IBaseService baseService,
            IDepartmentService departmentService,
            IConfiguration configuration) : base(sessionService)
        {
            this.employeeService = employeeService;
            this.reportService = reportService;
            this.teamService = teamService;
            this.baseService = baseService;
            this.departmentService = departmentService;
            isEditableSettingCheckInOut = configuration.GetValue<bool>("IsEditableSettingCheckInOut");
        }

        public async Task<IActionResult> Attendance(EmployeeAttendanceViewModel model)
        {
            const int numberItems = 500;
            var employeeAttendanceViewModel = model;
            if (employeeAttendanceViewModel != null)
            {
                if (employeeAttendanceViewModel.FromDate == DateTimeOffset.MinValue.DateTime)
                {
                    employeeAttendanceViewModel = new EmployeeAttendanceViewModel
                    {
                        FromDate = DateTimeOffset.Now.DateTime,
                        ToDate = DateTimeOffset.Now.DateTime,
                        DateOption = 0,
                        ExcludeUserDelete = true
                    };
                }

                employeeAttendanceViewModel.CurrentPage = Constants.CurrentPage;
                employeeAttendanceViewModel.ItemsPerPage = numberItems;
                employeeAttendanceViewModel.Teams = await teamService.GetTeam();
                employeeAttendanceViewModel.Departments = await departmentService.GetAll();
            }

            return View(employeeAttendanceViewModel);
        }

        public async Task<IActionResult> ListAttendance(EmployeeAttendanceViewModel model)
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
                    DepartmentId = model.DepartmentId,
                    ExcludeUserDelete = model.ExcludeUserDelete,
                    ItemsPerPage = model.ItemsPerPage,
                    PageIndex = model.CurrentPage - 1
                };

                attendances = await reportService.GetEmployeeAttendanceReport(criteria);
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

        public async Task<JsonResult> GetEmployeeAttendance(EmployeeAttendanceViewModel model)
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
                employeeAttendances = await reportService.GetEmployeeAttendanceReport(getEmployeeAttendanceReportCriteria);
            }

            return Json(employeeAttendances);
        }

        public async Task<IActionResult> Timesheet()
        {
            var model = new TimesheetViewModel
            {
                Year = DateTimeOffset.Now.Year,
                EmpId = CurrentUser.Id,
                IsHavePrivilegeCreateCheckInOutRecord = await IsPrivilegeCreateCheckInOut()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddChekInTime()
        {
            var criteria = new InsertCheckInOutRecordCriteria
            {
                EmpId = CurrentUser.Id,
                RecordTime = DateTimeOffset.Now.DateTime,
                InOutType = 0,
                IPAddress = GetClientIpAddress(),
                Platform = Request.Headers["User-Agent"]
            };
            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddChekOutTime()
        {
            var criteria = new InsertCheckInOutRecordCriteria
            {
                EmpId = CurrentUser.Id,
                RecordTime = DateTimeOffset.Now.DateTime,
                InOutType = 1,
                IPAddress = GetClientIpAddress(),
                Platform = Request.Headers["User-Agent"]
            };
            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> EditChekInOutTime(DateTime attendanceDate, string time, short inOutType)
        {
            var dateTime = attendanceDate.ToString("dd/MM/yyyy");
            var parseTime = TimeSpan.ParseExact(time, @"h\:mm", CultureInfo.InvariantCulture);
            DateTime recordTime = DateTime.ParseExact(dateTime, "dd/MM/yyyy", CultureInfo.InvariantCulture) + parseTime;
            var criteria = new EditInsertCheckInOutRecordCriteria
            {
                EmpId = CurrentUser.Id,
                RecordTime = recordTime,
                InOutType = inOutType,
                IPAddress = GetClientIpAddress(),
                Platform = Request.Headers["User-Agent"],
                ModifiedBy = CurrentUser.Id,
                ModifiedDate = DateTimeOffset.Now.DateTime,
                IsEditCheckInOutOnline = true
            };
            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);
            return Json(result);
        }

        public string GetClientIpAddress()
        {
            var clientIP = HttpContext.Request?.Headers["x-forwarded-for"].ToString();
            if (string.IsNullOrWhiteSpace(clientIP))
            {
                clientIP = HttpContext?.Connection?.RemoteIpAddress?.ToString();
            }

            return clientIP;
        }

        private async Task<bool> IsPrivilegeCreateCheckInOut()
        {
            var criteria = new GetPrivilegeCreateCheckInOutRecordCriteria
            {
                EmpId = CurrentUser.Id
            };

            var isPrivilegeCreateCheckInOut = await reportService.GetIsPrivilegeCreateCheckInOut(criteria);
            return isPrivilegeCreateCheckInOut;
        }

        [HttpPost]
        public async Task<IActionResult> ListTimesheet(TimesheetViewModel model)
        {
            IList<EmployeeAttendanceModel> employeeAttendances = new List<EmployeeAttendanceModel>();
            if (model != null)
            {
                var getEmployeeAttendanceReportCriteria = new GetEmployeeAttendanceReportByTypeCriteria
                {
                    FromDate = new DateTimeOffset(model.Year, model.Month, 1, 0, 0, 0, TimeSpan.Zero).DateTime,
                    ToDate = new DateTimeOffset(model.Year, model.Month, DateTime.DaysInMonth(model.Year, model.Month), 0, 0, 0, TimeSpan.Zero).DateTime,
                    ReportType = model.RequestType,
                    EmployeeId = model.EmpId
                };

                employeeAttendances = await reportService.GetEmployeeAttendanceReport(getEmployeeAttendanceReportCriteria);
                foreach (var attendance in employeeAttendances)
                {
                    var parseAttendanceDate = (DateTime)attendance.AttendanceDate;
                    var isOverOneWeek = DateTimeOffset.Now.DateTime >= parseAttendanceDate.AddDays(7);
                    attendance.IsWorkFromHome = attendance.IsWorkFromHome && isEditableSettingCheckInOut && !isOverOneWeek;
                }
            }

            return PartialView("_ListTimesheet", employeeAttendances);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Hr", Justification = "Reviewed")]
        [HrmAuthorize(UserRoles = "HR")]
        public IActionResult HrRequestMonitoring()
        {
            var currentDate = DateTimeOffset.Now.DateTime.ToClientTimeZone();
            var year = currentDate.Year;
            var month = currentDate.Month;

            var model = new RequestsMonitoringViewModel
            {
                FromDate = new DateTimeOffset(year, month, 1, 0, 0, 0, TimeSpan.Zero).DateTime,
                ToDate = new DateTimeOffset(year, month, DateTime.DaysInMonth(year, month), 0, 0, 0, TimeSpan.Zero).DateTime,
                RequestType = 0,
                MaxRequest = 2,
                DateOption = 0,
                IsLeader = false
            };

            return View("RequestMonitoring", model);
        }

        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> ListHRRequestMonitoring(RequestsMonitoringViewModel model)
        {
            IList<RequestsMonitoring> requestMonitorings = await GetRequestMonitoringDetail(model);

            return PartialView("_ListRequestMonitoring", requestMonitorings);
        }

        [HrmAuthorize(UserRoles = "AssistantLeader")]
        public IActionResult LeaderRequestMonitoring()
        {
            var year = DateTimeOffset.Now.Year;
            var month = DateTimeOffset.Now.Month;

            var model = new RequestsMonitoringViewModel
            {
                FromDate = new DateTimeOffset(year, month, 1, 0, 0, 0, TimeSpan.Zero).DateTime,
                ToDate = new DateTimeOffset(year, month, DateTime.DaysInMonth(year, month), 0, 0, 0, TimeSpan.Zero).DateTime,
                RequestType = 0,
                MaxRequest = 1,
                DateOption = 0,
                IsLeader = true
            };

            return View("RequestMonitoring", model);
        }

        [HrmAuthorize(UserRoles = "AssistantLeader")]
        public async Task<IActionResult> ListLeaderRequestMonitoring(RequestsMonitoringViewModel model)
        {
            return PartialView("_ListRequestMonitoring", await GetRequestMonitoringDetail(model));
        }

        private async Task<IList<RequestsMonitoring>> GetRequestMonitoringDetail(RequestsMonitoringViewModel model)
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

            IList<RequestsMonitoring> requestMonitorings = await reportService.GetRequestMonitoring(criteria);

            ViewBag.FromDate = model.FromDate;
            ViewBag.ToDate = model.ToDate;
            ViewBag.isLeader = model.IsLeader;
            ViewBag.MaxRequest = model.MaxRequest;

            return requestMonitorings;
        }

        public async Task<IActionResult> TotalWorkingHour()
        {
            IList<Team> teams = await teamService.GetTeam();
            var departments = await departmentService.GetAll();

            var model = new TotalWorkingHoursViewModel
            {
                FromDate = new DateTimeOffset(DateTimeOffset.Now.Year, DateTimeOffset.Now.Month, 1, 0, 0, 0, TimeSpan.Zero).DateTime,
                ToDate = DateTimeOffset.Now.DateTime,
                DateOption = 5,
                IsExportExcel = false,
                Teams = teams,
                Departments = departments
            };

            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> GetTotalWorkingHour(TotalWorkingHoursViewModel model)
        {
            const int dayOptions = 5;
            var totalWorkingHoursViewModel = model;
            if (totalWorkingHoursViewModel == null)
            {
                int year = DateTimeOffset.Now.Year;
                int month = DateTimeOffset.Now.Month;

                totalWorkingHoursViewModel = new TotalWorkingHoursViewModel
                {
                    FromDate = new DateTimeOffset(year, month, 1, 0, 0, 0, TimeSpan.Zero).DateTime,
                    ToDate = DateTimeOffset.Now.DateTime,
                    DateOption = dayOptions,
                    IsExportExcel = false
                };
            }

            var getWorkingHoursCriteria = new GetTotalWorkingHoursSummaryCriteria
            {
                FromDate = totalWorkingHoursViewModel.FromDate,
                ToDate = totalWorkingHoursViewModel.ToDate,
                EmployeeId = (totalWorkingHoursViewModel.EmployeeId > 0) ? new int?(totalWorkingHoursViewModel.EmployeeId) : null,
                GroupId = totalWorkingHoursViewModel.TeamId,
                DepartmentId = totalWorkingHoursViewModel.DepartmentId
            };

            IList<TotalWorkingHours> totalWorkingHours = await reportService.GetTotalWorkingHours(getWorkingHoursCriteria);

            IList<TotalWorkingHours> totalWorkingHour = SortTotalWorking(model?.Sort, totalWorkingHours);

            return Json(totalWorkingHour);
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

        public async Task<IActionResult> LateInEarlyOut(int empId = 0, string nickname = "")
        {
            IList<Team> teams = await teamService.GetTeam();

            var model = empId != 0
                ? new LateInEarlyOutViewModel
                {
                    Year = DateTimeOffset.Now.Year,
                    Teams = teams,
                    EmployeeId = empId,
                    Employee = nickname
                }
                : new LateInEarlyOutViewModel
                {
                    Year = DateTimeOffset.Now.Year,
                    Teams = teams,
                };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ListLateInEarlyOut(LateInEarlyOutViewModel model)
        {
            IEnumerable<LateInEarlyOutSummary> lateInEarlyOuts = new List<LateInEarlyOutSummary>();
            if (model != null)
            {
                var criteria = new GetLateInEarlyOutReportCriteria
                {
                    EmployeeId = model.EmployeeId > 0 ? new int?(model.EmployeeId) : null,
                    GroupId = model.TeamId,
                    Year = model.Year
                };

                lateInEarlyOuts = await reportService.GetLateInEarlyOut(criteria);

                ViewBag.Year = model.Year;
            }

            return PartialView("_ListLateInEarlyOut", lateInEarlyOuts);
        }

        [HttpGet]
        public async Task<IActionResult> LateInEarlyOutDetail(
            DateTime fromDate,
            DateTime toDate,
            int? employeeId = null,
            int? teamId = null)
        {
            IList<Team> teams = await teamService.GetTeam();

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
                Employee employee = await employeeService.GetEmployeeById(getEmployeeCriteria);
                model.Employee = employee.DisplayName;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> GetLateInEarlyOutDetail(LateInEarlyOutDetailViewModel model)
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

                lateInEarlyOuts = await reportService.GetLateInEarlyOutDetail(criteria);
            }

            return Json(lateInEarlyOuts);
        }

        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> LateInEarlyOutDiscipline()
        {
            IList<Team> teams = await teamService.GetTeam();
            var today = DateTimeOffset.Now.DateTime.ToClientTimeZone();

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

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> ListLateInEarlyOutDiscipline(LateInEarlyOutSummaryViewModel model)
        {
            IList<LateInEarlyOutDiscipline> lateInEarlyOutDisciplines = new List<LateInEarlyOutDiscipline>();
            if (model != null)
            {
                var toDate = new DateTimeOffset(model.ToYear, model.ToMonth, DateTime.DaysInMonth(model.ToYear, model.ToMonth), 0, 0, 0, TimeSpan.Zero).DateTime;
                var criteria = new GetLateInEarlyOutSummaryCriteria
                {
                    EmployeeId = model.EmployeeId > 0 ? new int?(model.EmployeeId) : null,
                    GroupId = model.TeamId,
                    FromDate = toDate.AddYears(-1).AddDays(1),
                    ToDate = new DateTimeOffset(model.ToYear, model.ToMonth, DateTime.DaysInMonth(model.ToYear, model.ToMonth), 0, 0, 0, TimeSpan.Zero).DateTime,
                    IsUserWarning = true
                };

                lateInEarlyOutDisciplines = await reportService.GetLateInEarlyOutDiscipline(criteria);
            }

            return PartialView("_ListLateInEarlyOutDiscipline", lateInEarlyOutDisciplines);
        }

        public IActionResult MyTimeRecord(DateTime? fromDate, DateTime? toDate)
        {
            DateTime from;
            DateTime to;
            if (!fromDate.HasValue || !toDate.HasValue)
            {
                from = new DateTimeOffset(DateTimeOffset.Now.Year, DateTimeOffset.Now.Month, 1, 0, 0, 0, TimeSpan.Zero).DateTime;
                to = DateTimeOffset.Now.Date;
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

        public async Task<IActionResult> ListMyTimeRecord(TimeRecordViewModel model)
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
                timeRecords = await reportService.GetTimeRecords(criteria);

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

        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> TimeRecord(DateTime? fromDate, DateTime? toDate)
        {
            DateTime from;
            DateTime to;

            if (!fromDate.HasValue || !toDate.HasValue)
            {
                from = DateTimeOffset.Now.Date;
                to = DateTimeOffset.Now.Date;
            }
            else
            {
                from = fromDate.Value;
                to = toDate.Value;
            }

            var teams = await teamService.GetTeam();

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

        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> ListTimeRecord(TimeRecordViewModel model)
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

                timeRecords = await reportService.GetTimeRecords(criteria);

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

        [HttpPost]
        public async Task<JsonResult> GetWorkingHoursStatistic(WorkingHoursStatisticViewModel model)
        {
            var getWorkingHoursCriteria = new GetWorkingHoursStatisticCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                FromDateLastMonth = model.FromDateLastMonth,
                ToDateLastMonth = model.ToDateLastMonth,
                EmployeeId = (model.EmployeeId > 0) ? new int?(model.EmployeeId) : null,
                GroupId = model.TeamId,
                DepartmentId = model.DepartmentId
            };

            IList<WorkingHoursStatistic> totalWorkingHours = await reportService.GetWorkingHoursStatistic(getWorkingHoursCriteria);

            return Json(totalWorkingHours);
        }

        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> WorkingHoursStatistic()
        {
            var teams = await teamService.GetTeam();
            var departments = await departmentService.GetAll();
            var today = DateTimeOffset.Now.DateTime.ToClientTimeZone();
            DateTime date = new DateTimeOffset(today.Year, today.Month, 1, 0, 0, 0, TimeSpan.Zero).DateTime;
            var model = new WorkingHoursStatisticViewModel
            {
                FromDate = date,
                ToDate = date.AddMonths(1).AddDays(-1),
                FromDateLastMonth = date.AddMonths(-1).AddDays(25),
                ToDateLastMonth = date.AddDays(-1),
                DateOption = 0,
                IsExportExcel = false,
                Teams = teams,
                Departments = departments,
                Month = date.Month,
                Year = date.Year
            };
            return View(model);
        }

        [HrmAuthorize(UserRoles = "HR")]
        public IActionResult HeadCount()
        {
            return View();
        }

        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> GetHeadCount(int year)
        {
            var critera = new GetHeadCountByYearCriteria
            {
                StartDateOfYear = new DateTimeOffset(year, 1, 1, 0, 0, 0, TimeSpan.Zero).DateTime,
                EndDateOfYear = new DateTimeOffset(year, 12, 31, 0, 0, 0, TimeSpan.Zero).DateTime
            };

            var model = new HeadCountModel
            {
                StartDate = critera.StartDateOfYear,
                EndDate = critera.EndDateOfYear,
                HeadCount = await reportService.GetTeamHeadCountInYear(critera)
            };
            return PartialView("_HeadCountTable", model);
        }

        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> YearlyNexRequest()
        {
            IList<Team> teams = await teamService.GetTeam();
            var model = new YearlyNexRequestViewModel
            {
                Teams = teams,
                Year = DateTimeOffset.Now.Year
            };

            return View(model);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> GetYearlyNexRequest(int? empId, int? teamId, int year)
        {
            int? employeeId = null;
            if (empId.HasValue && empId.Value > 0)
            {
                employeeId = empId;
            }

            var criteria = new GetYearlyNexRequestCriteria
            {
                EmployeeId = employeeId,
                TeamId = teamId,
                Year = year
            };

            ViewBag.Year = year;
            var yearlyNexRequests = await baseService.GetDataAsync<YearlyNexRequest>(criteria);

            return PartialView("_ListYearlyNexRequest", yearlyNexRequests);
        }
    }
}