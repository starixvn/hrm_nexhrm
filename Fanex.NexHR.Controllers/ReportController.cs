// --------------------------------------------------------------------------
// <copyright file="ReportController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
//   <change who="Josh.Phan" date="2014.03.11">Apply NAP Framework</change>
// </history>
namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Report controller.
    /// </summary>
    [Authorize]
    public class ReportController : BaseController
    {
        private readonly IEmployeeService _employeeService;
        private readonly IReportRepository _reportRepository;
        private readonly ITeamService _teamService;

        public ReportController(
            IEmployeeService employeeService = null,
            IReportRepository reportRepository = null,
            ITeamService teamService = null)
        {
            _employeeService = employeeService ?? new EmployeeService();
            _reportRepository = reportRepository ?? new ReportRepository();
            _teamService = teamService ?? new TeamService();
        }

        public ReportController()
            : this(null, null, null)
        {
        }

        /// <summary>
        /// Attendance report.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>Action result.</returns>
        public ActionResult Attendance(EmployeeAttendanceViewModel model)
        {
            var employee = this.CurrentUser;
            ViewBag.Groups = _teamService.GetAll(new GetAllTeamCriteria());

            if (model.FromDate == DateTime.MinValue)
            {
                model = new EmployeeAttendanceViewModel
                {
                    FromDate = DateTime.Now,
                    ToDate = DateTime.Now,
                    GroupId = employee.Team.Id,
                    DateOption = 0,
                    ExcludeUserDelete = true,
                };
            }

            return View(model);
        }

        /// <summary>
        /// Timesheet report.
        /// </summary>
        /// <returns>Action result.</returns>
        public ActionResult Timesheet()
        {
            var model = new TimesheetViewModel
            {
                Year = DateTime.Now.Year,
                EmpId = CurrentUser.Id
            };
            return View(model);
        }

        /// <summary>
        /// Employee request monitoring in current month.
        /// </summary>
        /// <returns>Action result.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult HrRequestMonitoring()
        {
            return HrRequestMonitoring(null);
        }

        /// <summary>
        /// HR request monitoring report.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult HrRequestMonitoring(RequestsMonitoringViewModel model)
        {
            if (model == null)
            {
                var year = DateTime.Now.Year;
                var month = DateTime.Now.Month;
                model = new RequestsMonitoringViewModel
                {
                    FromDate = new DateTime(year, month, 1),
                    ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                    MaxRequest = 2,
                    DateOption = 0
                };
            }

            ViewBag.FromDate = model.FromDate;
            ViewBag.ToDate = model.ToDate;
            ViewBag.isLeader = false;

            ViewBag.EntityList = new List<RequestsMonitoring>();
            if (this.CurrentUser.Roles != null)
            {
                if (this.CurrentUser.Roles.IndexOf(((int)RolesEnum.HR).ToString()) > -1
                    || this.CurrentUser.Roles.IndexOf(((int)RolesEnum.SA).ToString()) > -1)
                {
                    var getRequestMonitorCriteria = new GetRequestMonitorReportCriteria
                    {
                        FromDate = model.FromDate,
                        ToDate = model.ToDate,
                        MaxRequest = model.MaxRequest,
                        Role = (int)RolesEnum.HR,
                        EmployeeId = this.CurrentUser.Id
                    };

                    ViewBag.EntityList = _reportRepository.GetRequestMonitoring(getRequestMonitorCriteria);
                }
            }

            return View("RequestMonitoring", model);
        }

        /// <summary>
        /// Leader request monitoring report.
        /// </summary>
        /// <returns>Action result.</returns>
        [Authorize(Roles = "AssistantLeader")]
        public ActionResult LeaderRequestMonitoring()
        {
            return LeaderRequestMonitoring(null);
        }

        /// <summary>
        /// Leader request monitoring report.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        [Authorize(Roles = "AssistantLeader")]
        public ActionResult LeaderRequestMonitoring(RequestsMonitoringViewModel model)
        {
            if (model == null)
            {
                var year = DateTime.Now.Year;
                var month = DateTime.Now.Month;
                model = new RequestsMonitoringViewModel
                {
                    FromDate = new DateTime(year, month, 1),
                    ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                    MaxRequest = 1,
                    DateOption = 0
                };
            }

            ViewBag.FromDate = model.FromDate;
            ViewBag.ToDate = model.ToDate;
            ViewBag.isLeader = true;
            ViewBag.EntityList = new List<RequestsMonitoring>();

            if (this.CurrentUser.Roles != null)
            {
                if (this.CurrentUser.Roles.IndexOf(((int)RolesEnum.AssistantLeader).ToString()) > -1
                    || this.CurrentUser.Roles.IndexOf(((int)RolesEnum.Leader).ToString()) > -1
                    || this.CurrentUser.Roles.IndexOf(((int)RolesEnum.SA).ToString()) > -1)
                {
                    var getRequestMonitorCriteria = new GetRequestMonitorReportCriteria
                    {
                        FromDate = model.FromDate,
                        ToDate = model.ToDate,
                        MaxRequest = model.MaxRequest,
                        Role = (int)RolesEnum.AssistantLeader,
                        EmployeeId = this.CurrentUser.Id
                    };

                    ViewBag.EntityList = _reportRepository.GetRequestMonitoring(getRequestMonitorCriteria);
                }
            }

            return View("RequestMonitoring", model);
        }

        /// <summary>
        /// Total working hours report.
        /// </summary>
        /// <returns>Action result.</returns>
        public ActionResult TotalWorkingHours()
        {
            return TotalWorkingHours(null);
        }

        /// <summary>
        /// Total working hours report.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        public ActionResult TotalWorkingHours(TotalWorkingHoursViewModel model)
        {
            if (model == null)
            {
                var year = DateTime.Now.Year;
                var month = DateTime.Now.Month;
                model = new TotalWorkingHoursViewModel
                {
                    FromDate = new DateTime(year, month, 1),
                    ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month)),
                    DateOption = 5,
                    IsExportExcel = false
                };
            }

            if (model.IsExportExcel)
            {
                var export = new ExportController();
                return export.TotalWorkingHours(model);
            }

            ViewBag.Groups = _teamService.GetAll(new GetAllTeamCriteria());
            ViewBag.TeamId = model.TeamId;
            var getWorkingHoursCriteria = new GetTotalWorkingHoursSummaryCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                EmployeeId = (model.EmployeeId != -1 & model.EmployeeId != 0) ? new int?(model.EmployeeId) : null,
                GroupId = model.TeamId
            };

            var totalWorkingHours = _reportRepository.GetWorkingHoursList(getWorkingHoursCriteria);

            ViewBag.EntityList = totalWorkingHours;

            return View(model);
        }

        /// <summary>
        /// Leave summary report.
        /// </summary>
        /// <returns>Action result.</returns>
        public ActionResult LeaveSummary()
        {
            return LeaveSummary(null);
        }

        /// <summary>
        /// Leave summary report.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        public ActionResult LeaveSummary(LeaveSummaryViewModel model)
        {
            if (model == null)
            {
                model = new LeaveSummaryViewModel
                {
                    Year = DateTime.Now.Year,
                    EmployeeId = -1
                };
            }

            var getAnnualLeaveCriteria = new GetAnnualLeaveSummaryReportCriteria
            {
                EmployeeId = model.EmployeeId != -1 ? new int?(model.EmployeeId) : null,
                GroupId = model.TeamId,
                Year = model.Year
            };
            IList<LeaveSummary> entityList = _reportRepository.GetAnnualLeaveBalanceReport(getAnnualLeaveCriteria);

            IList<Team> teams = new List<Team>();

            if (model.TeamId != null)
            {
                var criteria = new GetTeamByIdCriteria
                {
                    Id = (int)model.TeamId
                };
                Team team = _teamService.Get(criteria);
                teams.Add(team);
            }
            else
            {
                teams = _teamService.GetAll(new GetAllTeamCriteria());
            }

            ViewBag.Groups = _teamService.GetAll(new GetAllTeamCriteria());
            ViewBag.TeamId = model.TeamId;
            ViewBag.EntityList = entityList;

            IList<LeaveSummaryByTeam> leaveSummaryByTeamList = _employeeService.GetAnnualLeaveByTeam(entityList, teams);

            ViewBag.LeaveSummaryByTeamList = leaveSummaryByTeamList;

            return View(model);
        }

        /// <summary>
        /// Extension number report.
        /// </summary>
        /// <returns>Action result.</returns>
        public ActionResult ExtensionNo()
        {
            return View();
        }
    }
}
