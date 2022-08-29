namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Core;
    using Core.Model;
    using Criteria.Calendar;
    using Criteria.Employee;
    using Criteria.Report;
    using Fanex.NexHR.Models.Attributes;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.Report;
    using Newtonsoft.Json;
    using Services;
    using Services.Interfaces;
    using ViewModels.Calendar;
    using ViewModels.Report;
    using ViewModels.TotalWorkingHours;

    public class ExportController : BaseController
    {
        private const string EmployeeAttendaceTemplatePath = "ExcelTemplates/EmployeeAttendace.xlsx";
        private const string EmployeeProfileTemplatePath = "ExcelTemplates/EmployeeProfile.xlsx";
        private const string TotalWorkingHoursTemplatePath = "ExcelTemplates/TotalWorkingHours.xlsx";
        private const string WorkingHoursStatisticTemplatePath = "ExcelTemplates/WorkingHoursStatistic.xlsx";
        private const string DayOffScheduleTemplatePath = "ExcelTemplates/DayOffSchedule.xlsx";
        private const string HeadCountReportTemplatePath = "ExcelTemplates/HeadCountReport.xlsx";

        private readonly IEmployeeService employeeService;
        private readonly IReportService reportService;
        private readonly ICalendarService calendarService;
        private readonly IWebHostEnvironment environment;

        public ExportController(ISessionService sessionService,
            IEmployeeService employeeService,
            IReportService reportService,
            ICalendarService calendarService,
            IWebHostEnvironment environment) : base(sessionService)
        {
            this.employeeService = employeeService;
            this.reportService = reportService;
            this.calendarService = calendarService;
            this.environment = environment;
        }

        public async Task<IActionResult> TotalWorkingHours(TotalWorkingHoursViewModel model)
        {
            const int dateOption = 5;
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;

            var totalWorkingHoursViewModel = model;

            totalWorkingHoursViewModel ??= new TotalWorkingHoursViewModel
            {
                FromDate = new DateTime(year, month, 1),
                ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                DateOption = dateOption,
            };

            var getWorkingHoursCriteria = new GetTotalWorkingHoursSummaryCriteria
            {
                FromDate = totalWorkingHoursViewModel.FromDate,
                ToDate = totalWorkingHoursViewModel.ToDate,
                EmployeeId = (totalWorkingHoursViewModel.EmployeeId != -1 && totalWorkingHoursViewModel.EmployeeId != 0)
                    ? new int?(totalWorkingHoursViewModel.EmployeeId)
                    : null,
                GroupId = totalWorkingHoursViewModel.TeamId
            };

            IList<TotalWorkingHours> listTotalWorkingHours = await reportService.GetTotalWorkingHours(getWorkingHoursCriteria);

            if (model != null && model.Sort != null)
            {
                var sorts = JsonConvert.DeserializeObject<List<SortModel>>(model.Sort);

                listTotalWorkingHours = Utilities.MultipleSort(listTotalWorkingHours, sorts);
            }

            string fileName = "TotalWorkingHours_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".xlsx";
            string templatePath = Path.Combine(environment.ContentRootPath, TotalWorkingHoursTemplatePath);
            var bytes = ExportService.ExportTotalWorkingHours(listTotalWorkingHours, totalWorkingHoursViewModel.FromDate, totalWorkingHoursViewModel.ToDate, templatePath);

            return File(bytes, "application/xlsx", fileName);
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpGet]
        public async Task<IActionResult> EmployeeProfileAsync(
            string query = null,
            int? teamId = null,
            bool? isDisable = null,
            bool? isDelete = null,
            int? jobTitleId = null,
            string sort = null)
        {
            var getAllEmployeeCriteria = new GetEmployeeCriteria
            {
                Query = query ?? string.Empty,
                TeamId = teamId == -1 ? null : teamId,
                JobTitleId = jobTitleId,
                IsDisable = isDisable,
                IsDelete = isDelete,
                PageNumber = 1,
                PageSize = int.MaxValue,
                TotalRow = 2000
            };

            IList<Employee> listEmployees = await employeeService.Query(getAllEmployeeCriteria);

            const string fileName = "EmployeeProfile.xlsx";

            if (sort != null)
            {
                var sorts = JsonConvert.DeserializeObject<List<SortModel>>(sort);

                listEmployees = Utilities.MultipleSort(listEmployees, sorts);
            }

            string templatePath = Path.Combine(environment.ContentRootPath, EmployeeProfileTemplatePath);
            var bytes = ExportService.ExportEmployeeProfile(listEmployees, templatePath);

            return File(bytes, "application/xlsx", fileName);
        }

        [HttpGet]
        public async Task<IActionResult> Attendance(
            DateTime fromDate,
            DateTime toDate,
            int? empId,
            int? groupId,
            short attendanceType,
            bool excludeUserDelete,
            string sort = null)
        {
            const string fileName = "EmployeeAttendance.xlsx";
            var getEmployeeAttendanceReportCriteria = new GetEmployeeAttendanceReportByTypeCriteria
            {
                FromDate = fromDate,
                ToDate = toDate,
                ReportType = attendanceType,
                EmployeeId = empId,
                GroupId = groupId == -1 ? null : groupId,
                ExcludeUserDelete = excludeUserDelete
            };

            IList<EmployeeAttendanceModel> employeeAttendances = await reportService.GetEmployeeAttendanceReport(getEmployeeAttendanceReportCriteria);

            if (sort != null)
            {
                var sorts = JsonConvert.DeserializeObject<List<SortModel>>(sort);

                employeeAttendances = Utilities.MultipleSort(employeeAttendances, sorts);
            }

            string templatePath = Path.Combine(environment.ContentRootPath, EmployeeAttendaceTemplatePath);
            var bytes = ExportService.ExportEmployeeAttendances(employeeAttendances, templatePath);

            return File(bytes, "application/xlsx", fileName);
        }

        public async Task<IActionResult> WorkingHoursStatistic(WorkingHoursStatisticViewModel model)
        {
            var getWorkingHoursCriteria = new GetWorkingHoursStatisticCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                FromDateLastMonth = model.FromDateLastMonth,
                ToDateLastMonth = model.ToDateLastMonth,
                EmployeeId = (model.EmployeeId != -1 && model.EmployeeId != 0) ? new int?(model.EmployeeId) : null,
                GroupId = model.TeamId
            };

            string fileName = "WorkingHoursStatistic_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".xlsx";
            IList<WorkingHoursStatistic> listTotalWorkingHours = await reportService.GetWorkingHoursStatistic(getWorkingHoursCriteria);
            string templatePath = Path.Combine(environment.ContentRootPath, WorkingHoursStatisticTemplatePath);
            var bytes = ExportService.ExportWorkingHoursStatistic(listTotalWorkingHours, getWorkingHoursCriteria.FromDateLastMonth, getWorkingHoursCriteria.ToDate, templatePath);

            return File(bytes, "application/xlsx", fileName);
        }

        public async Task<IActionResult> LeaveScheduleReport(WorkingCalendarViewModel model)
        {
            DateTime date = new DateTime(model.Year, model.Month, 1);
            var criteria = new GetLeaveScheduleCriteria
            {
                FromDate = date,
                ToDate = date.AddMonths(1).AddDays(-1),
                EmployeeId = model.EmployeeId < 0 ? (int?)null : model.EmployeeId,
                TeamId = model.TeamId
            };

            var schedules = await calendarService.GetLeaveSchedule(criteria);

            const string fileName = "DayOffSchedule.xlsx";
            string templatePath = Path.Combine(environment.ContentRootPath, DayOffScheduleTemplatePath);
            var bytes = ExportService.ExportLeaveSummaryScheduleReport(schedules, templatePath);

            return File(bytes, "application/xlsx", fileName);
        }

        public async Task<IActionResult> HeadCountExport(int year)
        {
            var criteria = new GetHeadCountByYearCriteria
            {
                StartDateOfYear = new DateTime(year, 1, 1),
                EndDateOfYear = new DateTime(year, 12, 31)
            };

            var headCount = await reportService.GetTeamHeadCountInYear(criteria);
            var report = new HeadCountModel
            {
                StartDate = criteria.StartDateOfYear,
                EndDate = criteria.EndDateOfYear,
                HeadCount = headCount
            };
            string fileName = string.Format("{0}{1}{2}", "HeadCount", report.StartDate.Year, ".xlsx");
            string templatePath = Path.Combine(environment.ContentRootPath, HeadCountReportTemplatePath);
            var bytes = ExportService.ExportHeadCountReport(report, templatePath);

            return File(bytes, "application/xlsx", fileName);
        }
    }
}