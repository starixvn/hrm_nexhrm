namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Core;
    using Core.Model;
    using Criteria.Calendar;
    using Criteria.Employee;
    using Criteria.Report;
    using Criteria.Team;
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
        private const string EmployeeAttendaceTemplatePath = @"~\App_Data\ExcelTemplates\EmployeeAttendace.xls";
        private const string EmployeeProfileTemplatePath = @"~\App_Data\ExcelTemplates\EmployeeProfile.xls";
        private const string TotalWorkingHoursTemplatePath = @"~\App_Data\ExcelTemplates\TotalWorkingHours.xls";
        private const string WorkingHoursStatisticTemplatePath = @"~\App_Data\ExcelTemplates\WorkingHoursStatistic.xls";
        private const string DayOffScheduleTemplatePath = @"~\App_Data\ExcelTemplates\DayOffSchedule.xls";
        private const string HeadCountReportTemplatePath = @"~\App_Data\ExcelTemplates\HeadCountReport.xls";

        private readonly IEmployeeService _employeeService;
        private readonly IReportService _reportService;
        private readonly ICalendarService _calendarService;

        public ExportController(
            EmployeeService employeeService,
            IReportService reportService,
            ICalendarService calendarService)
        {
            _employeeService = employeeService ?? new EmployeeService();
            _reportService = reportService ?? new ReportService();
            _calendarService = calendarService ?? new CalendarService();
        }

        public ExportController()
            : this(null, null, null)
        {
        }

        public ActionResult TotalWorkingHours(TotalWorkingHoursViewModel model)
        {
            const int dateOption = 5;
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;

            var totalWorkingHoursViewModel = model;

            if (totalWorkingHoursViewModel == null)
            {
                totalWorkingHoursViewModel = new TotalWorkingHoursViewModel
                {
                    FromDate = new DateTime(year, month, 1),
                    ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                    DateOption = dateOption,
                };
            }

            var getWorkingHoursCriteria = new GetTotalWorkingHoursSummaryCriteria
            {
                FromDate = totalWorkingHoursViewModel.FromDate,
                ToDate = totalWorkingHoursViewModel.ToDate,
                EmployeeId = (totalWorkingHoursViewModel.EmployeeId != -1 && totalWorkingHoursViewModel.EmployeeId != 0)
                    ? new int?(totalWorkingHoursViewModel.EmployeeId)
                    : null,
                GroupId = totalWorkingHoursViewModel.TeamId
            };

            IList<TotalWorkingHours> listTotalWorkingHours = _reportService.GetTotalWorkingHours(getWorkingHoursCriteria);

            if (model.Sort != null)
            {
                var sorts = JsonConvert.DeserializeObject<List<SortModel>>(model.Sort);

                listTotalWorkingHours = Utilities.MultipleSort(listTotalWorkingHours, sorts);
            }

            string templatePath = Server.MapPath(TotalWorkingHoursTemplatePath);

            ExportService.ExportTotalWorkingHours(listTotalWorkingHours, totalWorkingHoursViewModel.FromDate, totalWorkingHoursViewModel.ToDate, templatePath);

            return new EmptyResult();
        }

        [ValidateInput(false)]
        [Authorize(Roles = "HR")]
        [HttpGet]
        public ActionResult EmployeeProfile(
            string query = null,
            int? teamId = null,
            bool? enable = null,
            bool? isDelete = null,
            int? jobTitleId = null,
            string sort = null)
        {
            var getAllEmployeeCriteria = new GetEmployeeCriteria
            {
                Query = query,
                TeamId = teamId == -1 ? null : teamId,
                JobTitleId = jobTitleId,
                EnableStatus = enable,
                DeleteStatus = isDelete,
                PageNumber = 1,
                PageSize = int.MaxValue,
                TotalRow = 2000
            };

            IList<Employee> listEmployees = _employeeService.Query(getAllEmployeeCriteria);

            if (sort != null)
            {
                var sorts = JsonConvert.DeserializeObject<List<SortModel>>(sort);

                listEmployees = Utilities.MultipleSort(listEmployees, sorts);
            }

            string templatePath = Server.MapPath(EmployeeProfileTemplatePath);
            ExportService.ExportEmployeeProfile(listEmployees, templatePath);

            return new EmptyResult();
        }

        [HttpGet]
        public ActionResult Attendance(
            DateTime fromDate,
            DateTime toDate,
            int? empId,
            int? groupId,
            short attendanceType,
            bool excludeUserDelete,
            string sort = null)
        {
            var getEmployeeAttendanceReportCriteria = new GetEmployeeAttendanceReportByTypeCriteria
            {
                FromDate = fromDate,
                ToDate = toDate,
                ReportType = attendanceType,
                EmployeeId = empId,
                GroupId = groupId == -1 ? null : groupId,
                ExcludeUserDelete = excludeUserDelete
            };

            IList<EmployeeAttendanceModel> employeeAttendances = _reportService.GetEmployeeAttendanceReport(getEmployeeAttendanceReportCriteria);

            if (sort != null)
            {
                var sorts = JsonConvert.DeserializeObject<List<SortModel>>(sort);

                employeeAttendances = Utilities.MultipleSort(employeeAttendances, sorts);
            }

            string templatePath = Server.MapPath(EmployeeAttendaceTemplatePath);
            ExportService.ExportEmployeeAttendances(employeeAttendances, templatePath);

            return new EmptyResult();
        }

        public ActionResult WorkingHoursStatistic(WorkingHoursStatisticViewModel model)
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

            IList<WorkingHoursStatistic> listTotalWorkingHours = _reportService.GetWorkingHoursStatistic(getWorkingHoursCriteria);
            string templatePath = Server.MapPath(WorkingHoursStatisticTemplatePath);

            ExportService.ExportWorkingHoursStatistic(listTotalWorkingHours, getWorkingHoursCriteria.FromDateLastMonth, getWorkingHoursCriteria.ToDate, templatePath);

            return new EmptyResult();
        }

        public ActionResult LeaveScheduleReport(WorkingCalendarViewModel model)
        {
            DateTime date = new DateTime(model.Year, model.Month, 1);
            var templatePath = Server.MapPath(DayOffScheduleTemplatePath);

            var criteria = new GetLeaveScheduleCriteria
            {
                FromDate = date,
                ToDate = date.AddMonths(1).AddDays(-1),
                EmployeeId = model.EmployeeId < 0 ? (int?)null : model.EmployeeId,
                TeamId = model.TeamId
            };

            var schedules = _calendarService.GetLeaveSchedule(criteria);

            ExportService.ExportLeaveSummaryScheduleReport(schedules, templatePath);

            return new EmptyResult();
        }

        public ActionResult HeadCountExport(int year)
        {
            var templatePath = Server.MapPath(HeadCountReportTemplatePath);
            var criteria = new GetHeadCountByYearCriteria
            {
                StartDateOfYear = new DateTime(year, 1, 1),
                EndDateOfYear = new DateTime(year, 12, 31)
            };

            var headCount = _reportService.GetTeamHeadCountInYear(criteria);
            var report = new HeadCountModel
            {
                StartDate = criteria.StartDateOfYear,
                EndDate = criteria.EndDateOfYear,
                HeadCount = headCount
            };
            ExportService.ExportHeadCountReport(report, templatePath);
            return new EmptyResult();
        }
    }
}