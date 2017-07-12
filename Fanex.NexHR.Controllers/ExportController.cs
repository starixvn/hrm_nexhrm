namespace Fanex.NexHR.Controllers
{
    using Core;
    using Core.Model;
    using Criteria.Employee;
    using Criteria.Report;
    using Models;
    using Models.Report;
    using Newtonsoft.Json;
    using Services;
    using Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using ViewModels.TotalWorkingHours;

    public class ExportController : BaseController
    {
        private const string EmployeeAttendaceTemplatePath = @"~\App_Data\ExcelTemplates\EmployeeAttendace.xls";
        private const string EmployeeProfileTemplatePath = @"~\App_Data\ExcelTemplates\EmployeeProfile.xls";
        private const string TotalWorkingHoursTemplatePath = @"~\App_Data\ExcelTemplates\TotalWorkingHours.xls";

        private readonly IEmployeeService _employeeService;
        private readonly IReportService _reportService;

        public ExportController(
            EmployeeService employeeService,
            IReportService reportService)
        {
            _employeeService = employeeService ?? new EmployeeService();
            _reportService = reportService ?? new ReportService();
        }

        public ExportController()
            : this(null, null)
        {
        }

        public ActionResult TotalWorkingHours(TotalWorkingHoursViewModel model)
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;

            var totalWorkingHoursViewModel = model;

            if (totalWorkingHoursViewModel == null)
            {
                totalWorkingHoursViewModel = new TotalWorkingHoursViewModel
                {
                    FromDate = new DateTime(year, month, 1),
                    ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                    DateOption = 5,
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
            bool excludeUserDelete)
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

            string templatePath = Server.MapPath(EmployeeAttendaceTemplatePath);
            ExportService.ExportEmployeeAttendances(employeeAttendances, templatePath);

            return new EmptyResult();
        }
    }
}