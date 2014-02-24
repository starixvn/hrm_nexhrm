// --------------------------------------------------------------------------
// <copyright file="AnnualLeaveBalanceController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Controllers.Context;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Annual leave balance controller.
    /// </summary>
    [Authorize(Roles = "HR")]
    public class AnnualLeaveBalanceController : BaseController
    {
        private readonly IEmployeeService _employeeService;
        private readonly ITeamService _teamService;
        private readonly IReportRepository _reportRepository;

        public AnnualLeaveBalanceController()
        {
            _employeeService = new EmployeeService();
            _teamService = new TeamService();
            _reportRepository = new ReportRepository();
        }

        public AnnualLeaveBalanceController(IEmployeeService employeeService, ITeamService teamService, IReportRepository reportRepository)
        {
            _employeeService = employeeService;
            _teamService = teamService;
            _reportRepository = reportRepository ?? new ReportRepository();
        }

        public ActionResult Index()
        {
            ViewBag.IsShowCloseBalance = AppSetting.IsShowCloseBalance;
            return Index(null);
        }

        /// <summary>
        /// Index page.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        public ActionResult Index(LeaveSummaryViewModel model)
        {
            ViewBag.IsShowCloseBalance = AppSetting.IsShowCloseBalance;

            if (model == null)
            {
                model = new LeaveSummaryViewModel
                {
                    Year = DateTime.Now.Year,
                    EmployeeId = -1
                };
            }

            IList<LeaveSummary> entityList = _reportRepository.GetAnnualLeaveBalanceReport(
                model.Year,
                empId: model.EmployeeId != -1 ? new int?(model.EmployeeId) : null,
                groupId: model.TeamId);

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
                teams = _teamService.GetAll();
            }

            ViewBag.Groups = _teamService.GetAll();
            ViewBag.TeamId = model.TeamId;
            ViewBag.EntityList = entityList;

            IList<LeaveSummaryByTeam> leaveSummaryByTeamList = _employeeService.GetAnnualLeaveByTeam(entityList, teams);

            foreach (LeaveSummaryByTeam item in leaveSummaryByTeamList)
            {
                item.LeaveSummaryInTeam = item.LeaveSummaryInTeam.Where(m => m.LeaveBalance >= model.MinAnnualBalance).ToList();
            }

            ViewBag.LeaveSummaryByTeamList = leaveSummaryByTeamList;

            return View(model);
        }

        [ChildActionOnly]
        public PartialViewResult MultipleInsert()
        {
            return PartialView(new AnnualLeaveBalanceMultiViewModel
            {
                AffectedFrom = DateTime.Now
            });
        }

        [ChildActionOnly]
        public PartialViewResult MultipleReset()
        {
            return PartialView(new AnnualLeaveBalanceMultiViewModel
            {
                AffectedFrom = DateTime.Now
            });
        }

        /// <summary>
        /// Employee annual leave balance.
        /// </summary>
        /// <param name="id">The employee ID.</param>
        /// <returns>Action result.</returns>
        public ActionResult Employee(int id = -1)
        {
            var model = new AnnualLeaveBalanceViewModel
            {
                EmployeeId = id,
                Year = DateTime.Now.Year,
                ExcludedRequestTrans = true
            };

            var employee = _employeeService.Get(id);

            if (employee != null)
            {
                ViewBag.Employee = employee;
                ViewBag.Data = _employeeService.GetAnnualLeaveTransactions(model.EmployeeId, model.Year, false);
            }
            
            return View(model);
        }

        /// <summary>
        /// Employee annual leave balance.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        public ActionResult Employee(AnnualLeaveBalanceViewModel model)
        {
            model.ExcludedRequestTrans = model.ExcludedRequestTrans ? false : true;

            ViewBag.Employee = _employeeService.Get(model.EmployeeId);
            ViewBag.Data = _employeeService.GetAnnualLeaveTransactions(
                model.EmployeeId,
                model.Year,
                model.ExcludedRequestTrans);

            return View(model);
        }

        /// <summary>
        /// Edit employee annual leave balance.
        /// </summary>
        /// <param name="id">Annual leave transaction ID.</param>
        /// <returns>Action result.</returns>
        public ActionResult Edit(int id = -1)
        {
            var model = _employeeService.GetAnnualLeaveTransaction(id);
            if (model != null)
            {
                ViewBag.Employee = _employeeService.Get(model.EmpId);
            }

            return View(model);
        }

        /// <summary>
        /// Edit employee annual leave balance.
        /// </summary>
        /// <param name="formCollection">The form collection.</param>
        /// <param name="model">The model object.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        public ActionResult Edit(FormCollection formCollection, AnnualLeaveBalanceModel model)
        {
            ViewBag.Employee = _employeeService.Get(model.EmpId);

            if (formCollection["command"] == Resources.Labels.Delete)
            {
                var result = _employeeService.DeleteAnnualLeaveTransaction(model.Id, DateTime.Now, this.CurrentUser.Id);

                if (result)
                {
                    return RedirectToAction("Employee", "AnnualLeaveBalance", new { id = model.EmpId });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Resources.Messages.ErrorTryAgain);
                    return View(model);
                }
            }

            if (ModelState.IsValid)
            {
                // update annual leave here
                ViewBag.Updated = _employeeService.UpdateAnnualLeaveTransaction(
                    model.Id, 
                    model.Description,
                    model.AffectedFrom, 
                    model.DeductDays, 
                    model.CreditDays, 
                    DateTime.Now, 
                    this.CurrentUser.Id);
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return View(model);
        }

        /// <summary>
        /// Create employee annual leave balance.
        /// </summary>
        /// <param name="id">Annual leave balance ID.</param>
        /// <returns>Action result.</returns>
        public ActionResult Create(int id = -1)
        {
            ViewBag.Employee = _employeeService.Get(id);
            return View(new AnnualLeaveBalanceModel
            {
                EmpId = id,
                AffectedFrom = DateTime.Now
            });
        }

        /// <summary>
        /// Create employee annual leave balance.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>Action result.</returns>
        [HttpPost]
        public ActionResult Create(AnnualLeaveBalanceModel model)
        {
            ViewBag.Employee = _employeeService.Get(model.EmpId);

            if (ModelState.IsValid)
            {
                ViewBag.Inserted = _employeeService.InsertAnnualLeaveTransaction(
                    model.EmpId,
                    model.Description, 
                    model.AffectedFrom, 
                    model.DeductDays, 
                    model.CreditDays, 
                    DateTime.Now, 
                    this.CurrentUser.Id);
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return View(model);
        }
    }
}