// --------------------------------------------------------------------------
// <copyright file="ReportController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers.ApiControllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
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

        public IList<EmployeeAttendanceModel> GetEmployeeAttendances([FromUri]EmployeeAttendanceViewModel model)
        {
            if (model.GroupId == -1)
            {
                model.GroupId = null;
            }

            return _reportRepository.GetEmployeeAttendanceReport(model.FromDate, model.ToDate, model.Type, model.EmpId, model.GroupId, model.ExcludeUserDelete ? 1 : 0);
        }

        [HttpGet]
        [HttpPost]
        public IList<EmployeeAttendanceModel> Timesheet([FromUri]TimesheetViewModel model)
        {
            var fromDate = new DateTime(model.Year, model.Month, 1);
            var toDate = new DateTime(model.Year, model.Month, DateTime.DaysInMonth(model.Year, model.Month));
            return _reportRepository.GetEmployeeAttendanceReport(fromDate, toDate, model.Type, model.EmpId);
        }
    }
}
