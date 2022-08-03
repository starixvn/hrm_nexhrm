namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using Core;
    using Core.Enums;
    using Criteria.Employee;
    using Criteria.Report;
    using Fanex.NexHR.Core.Model;
    using Fanex.NexHR.Models.Attributes;
    using Fanex.NexHR.ViewModels.YearlyNexRequest;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
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
        private readonly ICommandService commandService;

        private static readonly bool isEditableSettingCheckInOut = bool.Parse(ConfigurationManager.AppSettings["IsEditableSettingCheckInOut"]);

        public ReportController(ISessionService sessionService,
            IEmployeeService employeeService,
            IReportService reportService,
            ITeamService teamService,
            ICommandService commandService) : base(sessionService)
        {
            this.employeeService = employeeService;
            this.reportService = reportService;
            this.teamService = teamService;
            this.commandService = commandService;
        }

        public ActionResult Attendance(EmployeeAttendanceViewModel model)
        {
            const int numberItems = 500;
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
                employeeAttendanceViewModel.Teams = teamService.GetTeam();
            }

            return View(employeeAttendanceViewModel);
        }

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
                employeeAttendances = reportService.GetEmployeeAttendanceReport(getEmployeeAttendanceReportCriteria);
            }

            return Json(employeeAttendances);
        }

        public ActionResult Timesheet()
        {
            var model = new TimesheetViewModel
            {
                Year = DateTime.Now.Year,
                EmpId = CurrentUser.Id,
                IsHavePrivilegeCreateCheckInOutRecord = IsPrivilegeCreateCheckInOut()
            };

            return View(model);
        }

        [HttpPost]
        public JsonResult AddChekInTime()
        {
            var criteria = new InsertCheckInOutRecordCriteria
            {
                EmpId = CurrentUser.Id,
                RecordTime = DateTime.Now,
                InOutType = 0,
                IPAddress = GetClientIpAddress(),
                Platform = Request.Headers["User-Agent"]
            };
            CommandResult result = commandService.ExecuteNonQuery(criteria);
            return Json(result);
        }

        [HttpPost]
        public JsonResult AddChekOutTime()
        {
            var criteria = new InsertCheckInOutRecordCriteria
            {
                EmpId = CurrentUser.Id,
                RecordTime = DateTime.Now,
                InOutType = 1,
                IPAddress = GetClientIpAddress(),
                Platform = Request.Headers["User-Agent"]
            };
            CommandResult result = commandService.ExecuteNonQuery(criteria);
            return Json(result);
        }


        [HttpPost]
        public JsonResult EditChekInOutTime(DateTime attendanceDate, string time, short inOutType)
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
                ModifiedDate = DateTime.Now,
                IsEditCheckInOutOnline = true
            };
            CommandResult result = commandService.ExecuteNonQuery(criteria);
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

        private bool IsPrivilegeCreateCheckInOut()
        {
            var criteria = new GetPrivilegeCreateCheckInOutRecordCriteria
            {
                EmpId = CurrentUser.Id
            };

            var isPrivilegeCreateCheckInOut = reportService.GetIsPrivilegeCreateCheckInOut(criteria);
            return isPrivilegeCreateCheckInOut;
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

                employeeAttendances = reportService.GetEmployeeAttendanceReport(getEmployeeAttendanceReportCriteria);
                foreach(var attendance in employeeAttendances)
                {
                    var parseAttendanceDate = (DateTime)attendance.AttendanceDate;
                    var isOverOneWeek = DateTime.Now >= parseAttendanceDate.AddDays(7);
                    attendance.IsWorkFromHome = attendance.IsWorkFromHome && isEditableSettingCheckInOut && !isOverOneWeek;
                }    
            }

            return PartialView("_ListTimesheet", employeeAttendances);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Hr", Justification = "Reviewed")]
        [HrmAuthorize(Roles = "HR")]
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

        [HrmAuthorize(Roles = "HR")]
        public ActionResult ListHRRequestMonitoring(RequestsMonitoringViewModel model)
        {
            IList<RequestsMonitoring> requestMonitorings = GetRequestMonitoringDetail(model);

            return PartialView("_ListRequestMonitoring", requestMonitorings);
        }

        [HrmAuthorize(Roles = "AssistantLeader")]
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

        [HrmAuthorize(Roles = "AssistantLeader")]
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

            IList<TotalWorkingHours> totalWorkingHours = reportService.GetTotalWorkingHours(getWorkingHoursCriteria);

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

        public ActionResult LateInEarlyOut(int empId = 0, string nickname = "")
        {
            IList<Team> teams = teamService.GetTeam();

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
                var criteria = new GetLateInEarlyOutDetailReportCriteria
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

        [HrmAuthorize(Roles = "HR")]
        public ActionResult LateInEarlyOutDiscipline()
        {
            IList<Team> teams = teamService.GetTeam();
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

        [HrmAuthorize(Roles = "HR")]
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
                timeRecords = reportService.GetTimeRecords(criteria);

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

        [HrmAuthorize(Roles = "HR")]
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

            IList<Team> teams = teamService.GetTeam();

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

        [HrmAuthorize(Roles = "HR")]
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

                timeRecords = reportService.GetTimeRecords(criteria);

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

            IList<WorkingHoursStatistic> totalWorkingHours = reportService.GetWorkingHoursStatistic(getWorkingHoursCriteria);

            return Json(totalWorkingHours);
        }

        [HrmAuthorize(Roles = "HR")]
        public ActionResult WorkingHoursStatistic()
        {
            IList<Team> teams = teamService.GetTeam();

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

        [HrmAuthorize(Roles = "HR")]
        public ActionResult HeadCount()
        {
            return View();
        }

        [HrmAuthorize(Roles = "HR")]
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
                HeadCount = reportService.GetTeamHeadCountInYear(critera)
            };
            return PartialView("_HeadCountTable", model);
        }

        [HrmAuthorize(Roles = "HR")]
        public ActionResult YearlyNexRequest()
        {
            IList<Team> teams = teamService.GetTeam();
            var model = new YearlyNexRequestViewModel
            {
                Teams = teams,
                Year = DateTime.Now.Year
            };

            return View(model);
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public ActionResult GetYearlyNexRequest(int? empId, int? teamId, int year)
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
            IList<YearlyNexRequest> yearlyNexRequests = reportService.GetYearlyNexRequest(criteria);

            return PartialView("_ListYearlyNexRequest", yearlyNexRequests);
        }
    }
}