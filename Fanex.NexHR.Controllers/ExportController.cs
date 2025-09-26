namespace Fanex.NexHR.Controllers
{
    using Core;
    using Core.Model;
    using Criteria.Calendar;
    using Criteria.Employee;
    using Criteria.Report;
    using Fanex.NexHR.Controllers.Attributes;
    using Fanex.NexHR.Criteria.Applicant;
    using Fanex.NexHR.Criteria.Overtime;
    using Fanex.NexHR.Models.Applicant;
    using Fanex.NexHR.Models.Export;
    using Fanex.NexHR.Models.Overtime;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.Report;
    using Newtonsoft.Json;
    using Services;
    using Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
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
        private const string ApplicantsTemplatePath = "ExcelTemplates/ApplicantsReport.xlsx";
        private const string LateInEarlyOutReportTemplatePath = "ExcelTemplates/LateInEarlyOutReport.xlsx";
        private const string NexSummaryReportTemplatePath = "ExcelTemplates/NexSummary.xlsx";
        private const string ContentTyleFile = "application/xlsx";

        private readonly IEmployeeService employeeService;
        private readonly IReportService reportService;
        private readonly ICalendarService calendarService;
        private readonly IWebHostEnvironment environment;
        private readonly IApplicantService applicantService;
        private readonly IBaseService baseService;

        public ExportController(ISessionService sessionService,
            IApplicantService applicantService,
            IEmployeeService employeeService,
            IReportService reportService,
            ICalendarService calendarService,
            IWebHostEnvironment environment,
            IBaseService baseService) : base(sessionService)
        {
            this.applicantService = applicantService;
            this.employeeService = employeeService;
            this.reportService = reportService;
            this.calendarService = calendarService;
            this.environment = environment;
            this.baseService = baseService;
        }

        public async Task<IActionResult> TotalWorkingHours(TotalWorkingHoursViewModel model)
        {
            const int dateOption = 5;
            var now = DateTimeOffset.Now.DateTime.ToClientTimeZone();
            var year = now.Year;
            var month = now.Month;
            var totalWorkingHoursViewModel = model;

            totalWorkingHoursViewModel ??= new TotalWorkingHoursViewModel
            {
                FromDate = new DateTimeOffset(year, month, 1, 0, 0, 0, TimeSpan.Zero).DateTime,
                ToDate = new DateTimeOffset(year, month, DateTime.DaysInMonth(year, month), 0, 0, 0, TimeSpan.Zero).DateTime,
                DateOption = dateOption,
            };

            var getWorkingHoursCriteria = new GetTotalWorkingHoursSummaryCriteria
            {
                FromDate = totalWorkingHoursViewModel.FromDate,
                ToDate = totalWorkingHoursViewModel.ToDate,
                EmployeeId = (totalWorkingHoursViewModel.EmployeeId != -1 && totalWorkingHoursViewModel.EmployeeId != 0)
                    ? new int?(totalWorkingHoursViewModel.EmployeeId)
                    : null,
                GroupId = totalWorkingHoursViewModel.TeamId,
                DepartmentId = totalWorkingHoursViewModel.DepartmentId
            };

            IList<TotalWorkingHours> listTotalWorkingHours = await reportService.GetTotalWorkingHours(getWorkingHoursCriteria);

            if (model != null && model.Sort != null)
            {
                var sorts = JsonConvert.DeserializeObject<List<SortModel>>(model.Sort);

                listTotalWorkingHours = Utilities.MultipleSort(listTotalWorkingHours, sorts);
            }

            string fileName = "TotalWorkingHours_" + now.Day + now.Month + now.Year + ".xlsx";
            string templatePath = Path.Combine(environment.ContentRootPath, TotalWorkingHoursTemplatePath);
            var bytes = ExportService.ExportTotalWorkingHours(listTotalWorkingHours, totalWorkingHoursViewModel.FromDate, totalWorkingHoursViewModel.ToDate, templatePath);

            return File(bytes, ContentTyleFile, fileName);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpGet]
        public async Task<IActionResult> EmployeeProfileAsync(
            string query = null,
            int? teamId = null,
            int? departmentId = null,
            bool? isDisable = null,
            bool? isDelete = null,
            int? jobTitleId = null,
            string sort = null)
        {
            var getAllEmployeeCriteria = new GetEmployeeCriteria
            {
                Query = query ?? string.Empty,
                TeamId = teamId == -1 ? null : teamId,
                DepartmentId = departmentId == -1 ? null : departmentId,
                JobTitleId = jobTitleId,
                IsDisable = isDisable,
                IsDelete = isDelete,
                IsSpecial = false,
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

            return File(bytes, ContentTyleFile, fileName);
        }

        [HttpGet]
        public async Task<IActionResult> Attendance(AttendanceExportModel model)
        {
            const string fileName = "EmployeeAttendance.xlsx";
            var getEmployeeAttendanceReportCriteria = new GetEmployeeAttendanceReportByTypeCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                ReportType = model.AttendanceType,
                EmployeeId = model.EmpId,
                GroupId = model.GroupId == -1 ? null : model.GroupId,
                DepartmentId = model.DepartmentId == -1 ? null : model.DepartmentId,
                ExcludeUserDelete = model.ExcludeUserDelete
            };

            IList<EmployeeAttendanceModel> employeeAttendances = await reportService.GetEmployeeAttendanceReport(getEmployeeAttendanceReportCriteria);

            if (model.Sort != null)
            {
                var sorts = JsonConvert.DeserializeObject<List<SortModel>>(model.Sort);

                employeeAttendances = Utilities.MultipleSort(employeeAttendances, sorts);
            }

            string templatePath = Path.Combine(environment.ContentRootPath, EmployeeAttendaceTemplatePath);
            var isHR = CurrentUser.HasHRRole;
            var bytes = ExportService.ExportEmployeeAttendances(employeeAttendances, templatePath, isHR);

            return File(bytes, ContentTyleFile, fileName);
        }

        [HttpGet]
        public async Task<IActionResult> OvertimeSummary(int? empId, int? teamId, int? departmentId, int year)
        {
            const string fileName = "NexSummary.xlsx";

            var criteria = new OvertimeSummaryCriteria
            {
                EmployeeId = empId,
                TeamId = teamId,
                DepartmentId = departmentId,
                Year = year
            };
            var listSummary = await baseService.GetDataAsync<OvertimeSummary>(criteria);

            string templatePath = Path.Combine(environment.ContentRootPath, NexSummaryReportTemplatePath);
            var isHR = CurrentUser.HasHRRole;
            var bytes = ExportService.ExportOvertimeSummary(listSummary, year, templatePath, isHR);

            return File(bytes, ContentTyleFile, fileName);
        }

        public async Task<IActionResult> WorkingHoursStatistic(WorkingHoursStatisticViewModel model)
        {
            var now = DateTimeOffset.Now.DateTime.ToClientTimeZone();

            var getWorkingHoursCriteria = new GetWorkingHoursStatisticCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                FromDateLastMonth = model.FromDateLastMonth,
                ToDateLastMonth = model.ToDateLastMonth,
                EmployeeId = (model.EmployeeId != -1 && model.EmployeeId != 0) ? new int?(model.EmployeeId) : null,
                DepartmentId = model.DepartmentId,
                GroupId = model.TeamId,
                PayrollCompanyId = model.PayrollCompanyId
            };

            string fileName = "WorkingHoursStatistic_" + now.Day + now.Month + now.Year + ".xlsx";
            IList<WorkingHoursStatistic> listTotalWorkingHours = await reportService.GetWorkingHoursStatistic(getWorkingHoursCriteria);
            string templatePath = Path.Combine(environment.ContentRootPath, WorkingHoursStatisticTemplatePath);
            var bytes = ExportService.ExportWorkingHoursStatistic(listTotalWorkingHours, getWorkingHoursCriteria.FromDateLastMonth, getWorkingHoursCriteria.ToDate, templatePath);

            return File(bytes, ContentTyleFile, fileName);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpGet]
        public async Task<IActionResult> ApplicantsReport(
            DateTime? fromDate,
            DateTime? toDate,
            int status,
            int positionId,
            int pageNumber,
            int pageSize,
            string fullName = null)
        {
            var criteria = new GetAllApplicantCriteria
            {
                FullName = fullName,
                FromDate = fromDate,
                ToDate = toDate,
                Status = status,
                PositionId = positionId,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            IEnumerable<Applicant> listApplicants = await applicantService.GetAll(criteria);
            const string fileName = "ApplicantsReport.xlsx";
            string templatePath = Path.Combine(environment.ContentRootPath, ApplicantsTemplatePath);
            var bytes = ExportService.ExportApplicants(listApplicants, fromDate, toDate, pageNumber, pageSize, templatePath);

            return File(bytes, ContentTyleFile, fileName);
        }

        public async Task<IActionResult> LeaveScheduleReport(WorkingCalendarViewModel model)
        {
            DateTime date = new DateTimeOffset(model.Year, model.Month, 1, 0, 0, 0, TimeSpan.Zero).DateTime;
            var criteria = new GetLeaveScheduleCriteria
            {
                FromDate = date,
                ToDate = date.AddMonths(1).AddDays(-1),
                EmployeeId = model.EmployeeId < 0 ? (int?)null : model.EmployeeId,
                TeamId = model.TeamId,
                DepartmentId = model.DepartmentId
            };

            var schedules = await calendarService.GetLeaveSchedule(criteria);

            const string fileName = "DayOffSchedule.xlsx";
            string templatePath = Path.Combine(environment.ContentRootPath, DayOffScheduleTemplatePath);
            var bytes = ExportService.ExportLeaveSummaryScheduleReport(schedules, templatePath);

            return File(bytes, ContentTyleFile, fileName);
        }

        public async Task<IActionResult> HeadCountExport(int year)
        {
            var criteria = new GetHeadCountByYearCriteria
            {
                StartDateOfYear = new DateTimeOffset(year, 1, 1, 0, 0, 0, TimeSpan.Zero).DateTime,
                EndDateOfYear = new DateTimeOffset(year, 12, 31, 0, 0, 0, TimeSpan.Zero).DateTime
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

            return File(bytes, ContentTyleFile, fileName);
        }

        [HttpGet]
        public async Task<IActionResult> LateInEarlyOutReport(int year, int? teamId = null, int? employeeId = null)
        {
            var criteria = new GetLateInEarlyOutReportCriteria
            {
                EmployeeId = employeeId > 0 ? new int?(employeeId.Value) : null,
                GroupId = teamId,
                Year = year
            };

            var lateInEarlyOuts = await reportService.GetLateInEarlyOut(criteria);

            string fileName = $"LateInEarlyOutReport_{year}.xlsx";
            string templatePath = Path.Combine(environment.ContentRootPath, LateInEarlyOutReportTemplatePath);
            var isHRRole = CurrentUser.HasHRRole;
            var bytes = ExportService.ExportLateInEarlyOutReport(lateInEarlyOuts, year, templatePath, isHRRole);

            return File(bytes, ContentTyleFile, fileName);
        }
    }
}