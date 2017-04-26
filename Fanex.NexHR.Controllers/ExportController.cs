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
        private readonly ExportService _exportService;

        public ExportController(
            EmployeeService employeeService,
            ExportService exportService,
            IReportService reportService)
        {
            _employeeService = employeeService ?? new EmployeeService();
            _exportService = exportService ?? new ExportService();
            _reportService = reportService ?? new ReportService();
        }

        public ExportController()
            : this(null, null, null)
        {
        }

        public ActionResult TotalWorkingHours(TotalWorkingHoursViewModel model)
        {
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;

            if (model == null)
            {
                model = new TotalWorkingHoursViewModel
                {
                    FromDate = new DateTime(year, month, 1),
                    ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                    DateOption = 5,
                };
            }

            var getWorkingHoursCriteria = new GetTotalWorkingHoursSummaryCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                EmployeeId = (model.EmployeeId != -1 & model.EmployeeId != 0) ? new int?(model.EmployeeId) : null,
                GroupId = model.TeamId
            };

            IList<TotalWorkingHours> listTotalWorkingHours = _reportService.GetTotalWorkingHours(getWorkingHoursCriteria);
            string templatePath = Server.MapPath(TotalWorkingHoursTemplatePath);

            _exportService.ExportTotalWorkingHours(listTotalWorkingHours, model.FromDate, model.ToDate, templatePath);

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
            if (teamId == -1)
            {
                teamId = null;
            }

            var getAllEmployeeCriteria = new GetEmployeeCriteria()
            {
                Query = query,
                TeamId = teamId,
                JobTitleId = jobTitleId,
                Enable = CheckUserEnableCondition(enable),
                IsDelete = CheckDeletedUserCondition(isDelete),
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
            _exportService.ExportEmployeeProfile(listEmployees, templatePath);

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
            if (groupId == -1)
            {
                groupId = null;
            }

            var getEmployeeAttendanceReportCriteria = new GetEmployeeAttendanceReportByTypeCriteria
            {
                FromDate = fromDate,
                ToDate = toDate,
                ReportType = attendanceType,
                EmployeeId = empId,
                GroupId = groupId,
                ExcludeUserDelete = excludeUserDelete
            };

            IList<EmployeeAttendanceModel> employeeAttendances = _reportService.GetEmployeeAttendanceReport(getEmployeeAttendanceReportCriteria);

            string templatePath = Server.MapPath(EmployeeAttendaceTemplatePath);
            _exportService.ExportEmployeeAttendances(employeeAttendances, templatePath);

            return new EmptyResult();
        }

        private int? CheckUserEnableCondition(bool? enable)
        {
            int? disableUsers;
            if (enable == true)
            {
                disableUsers = 0;
            }
            else if (enable == false)
            {
                disableUsers = 1;
            }
            else
            {
                disableUsers = null;
            }

            return disableUsers;
        }

        private int? CheckDeletedUserCondition(bool? isDelete)
        {
            int? deleteUsers;
            if (isDelete == true)
            {
                deleteUsers = 1;
            }
            else if (isDelete == false)
            {
                deleteUsers = 0;
            }
            else
            {
                deleteUsers = null;
            }

            return deleteUsers;
        }
    }
}