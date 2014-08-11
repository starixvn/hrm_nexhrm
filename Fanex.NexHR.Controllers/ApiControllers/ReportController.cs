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
    using System.Linq;
    using System.Web.Http;
    using Fanex.NexHR.Criteria.ExtensionNo;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Report controller.
    /// </summary>
    public class ReportController : ApiController
    {
        private readonly IReportRepository _reportRepository;
        private readonly ITeamService _teamService;
        private readonly IExtensionNoService _extensionNoService;

        public ReportController()
        {
            _reportRepository = new ReportRepository();
            _teamService = new TeamService();
            _extensionNoService = new ExtensionNoService();
        }

        public ReportController(IReportRepository reportRepository, ITeamService teamService, IExtensionNoService extensionNoService)
        {
            _reportRepository = reportRepository;
            _teamService = teamService;
            _extensionNoService = extensionNoService;
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
                ReportType = model.Type,
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
                ReportType = model.Type,
                EmployeeId = model.EmpId
            };

            return _reportRepository.GetEmployeeAttendanceReport(getEmployeeAttendanceReportCriteria);
        }

        /// <summary>
        /// Method Get Group Extension.
        /// </summary>
        /// <param name="isInternal">Check is Internal or External group.</param>
        /// <param name="groupId">Group's id.</param>
        /// <returns>List of SelectListItem.</returns>
        [HttpGet]
        [HttpPost]
        public IList<System.Web.Mvc.SelectListItem> GetGroupExtension(bool isInternal, int? groupId)
        {
            if (isInternal)
            {
                var data = _teamService.GetAll(new GetAllTeamCriteria());
                var selectList = data.Select(x => new System.Web.Mvc.SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();
                return selectList;
            }
            else
            {
                var criteria = new GetAllExtensionGroupInfoCriteria
                {
                    IsInternal = isInternal,
                    GroupId = groupId
                };
                var data = _extensionNoService.GetAllExtensionGroupInfo(criteria);
                var selectList = data.Select(x => new System.Web.Mvc.SelectListItem
                {
                    Text = x.TeamName,
                    Value = x.TeamId.ToString()
                }).ToList();
                return selectList;
            }
        }
    }
}
