// --------------------------------------------------------------------------
// <copyright file="ReportController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
//   <change who="Josh.Phan" date="2014.03.11">Apply NAP Framework</change>
// </history>

namespace Fanex.NexHR.Controllers.ApiControllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Report controller.
    /// </summary>
    public class ReportController : ApiController
    {
        private readonly IReportRepository _reportRepository;

        public ReportController()
        {
            _reportRepository = new ReportRepository();
        }

        public ReportController(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        /// <summary>
        /// Get employee attendance report.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Employee attendance model.</returns>
        public IList<EmployeeAttendanceModel> GetEmployeeAttendances([FromUri]EmployeeAttendanceViewModel model)
        {
            if (model.GroupId == -1)
            {
                model.GroupId = null;
            }

            var getEmployeeAttendanceReportCriteria = new GetEmployeeAttendanceReportByTypeCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                Type = model.Type,
                EmployeeId = model.EmpId,
                GroupId = model.GroupId,
                ExcludeUserDelete = model.ExcludeUserDelete ? 1 : 0
            };

            return _reportRepository.GetEmployeeAttendanceReport(getEmployeeAttendanceReportCriteria);
        }

        /// <summary>
        /// The timesheet.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Employee attendance model.</returns>
        [HttpGet]
        [HttpPost]
        public IList<EmployeeAttendanceModel> Timesheet([FromUri]TimesheetViewModel model)
        {
            var getEmployeeAttendanceReportCriteria = new GetEmployeeAttendanceReportByTypeCriteria
            {
                FromDate = new DateTime(model.Year, model.Month, 1),
                ToDate = new DateTime(model.Year, model.Month, DateTime.DaysInMonth(model.Year, model.Month)),
                Type = model.Type,
                EmployeeId = model.EmpId
            };

            return _reportRepository.GetEmployeeAttendanceReport(getEmployeeAttendanceReportCriteria);
        }
    }
}
