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
    using System.IO;
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.Report;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using NPOI.HSSF.UserModel;

    /// <summary>
    /// Export controller.
    /// </summary>
    public class ExportController : BaseController
    {
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

        /// <summary>
        /// Export total working hours.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>Action result.</returns>
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

            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            string fileName = "TotalWorkingHours_" + DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + ".xls";

            var getWorkingHoursCriteria = new GetTotalWorkingHoursSummaryCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                EmployeeId = (model.EmployeeId != -1 & model.EmployeeId != 0) ? new int?(model.EmployeeId) : null,
                GroupId = model.TeamId
            };

            IList<TotalWorkingHours> listTotalWorkingHours = _reportService.GetTotalWorkingHours(getWorkingHoursCriteria);

            _exportService.ExportTotalWorkingHours(hssfworkbook, listTotalWorkingHours, model.FromDate, model.ToDate);

            var output = new MemoryStream();
            hssfworkbook.Write(output);

            // Return the result to the end user
            return File(output.ToArray(), "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// Export employee profile.
        /// </summary>
        /// <param name="query">String to query.</param>
        /// <param name="groupId">Employee group id.</param>
        /// <param name="enable">Is enabled.</param>
        /// <param name="isDelete">Is Deleted.</param>
        /// <param name="titleId">Job title id.</param>
        /// <returns>Action result.</returns>
        [HttpGet]
        public ActionResult EmployeeProfile(
            string query = null,
            int? groupId = null,
            bool? enable = null,
            bool? isDelete = null,
            int? titleId = null)
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

            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            string fileName = "EmployeeProfile.xls";

            IList<Employee> listEmployees = _employeeService.Query(getAllEmployeeCriteria);

            _exportService.ExportEmployeeProfile(hssfworkbook, listEmployees);

            MemoryStream output = new MemoryStream();

            hssfworkbook.Write(output);

            // Return the result to the end user
            return File(output.ToArray(), "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// Export employee attendance.
        /// </summary>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date.</param>
        /// <param name="empId">Employee id.</param>
        /// <param name="groupId">Employee group id.</param>
        /// <param name="attendanceType">Attendance type.</param>
        /// <param name="excludeUserDelete">Exclude user delete.</param>
        /// <returns>Action result.</returns>
        [HttpGet]
        public ActionResult Attendance(
            DateTime fromDate,
            DateTime toDate,
            int? empId,
            int? groupId,
            short attendanceType,
            bool excludeUserDelete)
        {
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            string fileName = "EmployeeAttendance.xls";

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

            IList<EmployeeAttendanceModel> listEmployeeAttendances = _reportService.GetEmployeeAttendanceReport(getEmployeeAttendanceReportCriteria);

            _exportService.ExportEmployeeAttendances(hssfworkbook, listEmployeeAttendances);

            MemoryStream output = new MemoryStream();

            hssfworkbook.Write(output);

            // Return the result to the end user
            return File(output.ToArray(), "application/vnd.ms-excel", fileName);
        }

        /// <summary>
        /// Method check User query condition.
        /// </summary>
        /// <param name="enable">Enable condition.</param>
        /// <returns>A value to query DB.</returns>
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

        /// <summary>
        /// Method check User query condition.
        /// </summary>
        /// <param name="isDelete">Is deleted.</param>
        /// <returns>A value to query DB.</returns>
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