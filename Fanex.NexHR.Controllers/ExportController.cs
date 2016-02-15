// --------------------------------------------------------------------------
// <copyright file="ExportController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
//   <change who="Josh.Phan" date="2014.03.11">Apply NAP Framework</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Core.Model;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using Newtonsoft.Json;

    public class ExportController : BaseController
    {
        private const string EmployeeAttendaceTemplatePath = @"~\App_Data\ExcelTemplates\EmployeeAttendace.xls";
        private const string EmployeeProfileTemplatePath = @"~\App_Data\ExcelTemplates\EmployeeProfile.xls";
        private const string TotalWorkingHoursTemplatePath = @"~\App_Data\ExcelTemplates\TotalWorkingHours.xls";

        private readonly IEmployeeService _employeeService;
        private readonly IReportService _reportService;
        private readonly ExportService _exportService;

        public ExportController(
            EmployeeService employeeService = null,
            ExportService exportService = null,
            IReportService reportService = null)
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

        [Authorize(Roles = "HR")]
        [HttpGet]
        public ActionResult EmployeeProfile(
            string query = null,
            int? groupId = null,
            bool? enable = null,
            bool? isDelete = null,
            int? titleId = null,
            string sort = null)
        {
            if (groupId == -1)
            {
                groupId = null;
            }

            var getAllEmployeeCriteria = new GetAllEmployeeCriteria()
            {
                Query = query,
                GroupId = groupId,
                TitleId = titleId,
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
                ExcludeUserDelete = excludeUserDelete ? 1 : 0
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