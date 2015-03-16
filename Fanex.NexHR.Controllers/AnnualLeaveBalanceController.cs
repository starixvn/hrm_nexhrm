// --------------------------------------------------------------------------
// <copyright file="AnnualLeaveBalanceController.cs" company="Nexcel Solutions Vietnam">
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
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
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
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed. Suppression is OK here.")]
        public ActionResult Index(LeaveSummaryViewModel model)
        {
            IList<Team> allTeam = _teamService.GetAll(new GetAllTeamCriteria());
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
            IList<Team> listTeam = new List<Team>();
            if (model.TeamId != null)
            {
                var criteria = new GetTeamByIdCriteria
                {
                    Id = (int)model.TeamId
                };
                Team team = _teamService.GetById(criteria);
                listTeam.Add(team);
            }
            else
            {
                listTeam = allTeam;
            }

            IList<LeaveSummaryByTeam> leaveSummaryByTeamList = _employeeService.GetAnnualLeaveByTeam(entityList, listTeam);
            if (model.MinAnnualBalance > 0)
            {
                foreach (LeaveSummaryByTeam item in leaveSummaryByTeamList)
                {
                    item.LeaveSummaryInTeam = item.LeaveSummaryInTeam.Where(m => m.LeaveBalance >= model.MinAnnualBalance).ToList();
                }
            }

            ShowOpenAndCloseAnnualBalance showOpenAndClose = GetOpenAndCloseAnnualBalance();
            ViewBag.ShowOpenAndCloseAnnualBalance = showOpenAndClose;
            ViewBag.Groups = allTeam;
            ViewBag.TeamId = model.TeamId;
            ViewBag.EntityList = entityList;
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

            var getEmployeeCriteria = new GetEmployeeCriteria() { Id = id };
            var employee = _employeeService.GetEmployeeById(getEmployeeCriteria);

            if (employee != null)
            {
                ViewBag.Employee = employee;
                var getAnnualLeaveTransactionCriteria = new GetAnnualLeaveTransactionCriteria 
                {
                    EmployeeId = model.EmployeeId,
                    Year = model.Year,
                    ExcludedRequestTransactions = false
                };

                ViewBag.Data = _employeeService.GetAnnualLeaveTransactions(getAnnualLeaveTransactionCriteria);
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
            var getEmployeeCriteria = new GetEmployeeCriteria() { Id = model.EmployeeId };
            ViewBag.Employee = _employeeService.GetEmployeeById(getEmployeeCriteria);
            var getAnnualLeaveTransactionCriteria = new GetAnnualLeaveTransactionCriteria
            {
                EmployeeId = model.EmployeeId,
                Year = model.Year,
                ExcludedRequestTransactions = model.ExcludedRequestTrans
            };

            ViewBag.Data = _employeeService.GetAnnualLeaveTransactions(getAnnualLeaveTransactionCriteria);

            return View(model);
        }

        /// <summary>
        /// Edit employee annual leave balance.
        /// </summary>
        /// <param name="id">Annual leave transaction ID.</param>
        /// <returns>Action result.</returns>
        public ActionResult Edit(int id = -1)
        {
            var getAnnualLeaveByIdCriteria = new GetAnnualLeaveByIdCriteria
            {
                Id = id
            };
            var model = _employeeService.GetAnnualLeaveTransactionById(getAnnualLeaveByIdCriteria);
            if (model != null)
            {
                var getEmployeeCriteria = new GetEmployeeCriteria() { Id = model.EmpId };
                ViewBag.Employee = _employeeService.GetEmployeeById(getEmployeeCriteria);
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
            var getEmployeeCriteria = new GetEmployeeCriteria() { Id = model.EmpId };
            ViewBag.Employee = _employeeService.GetEmployeeById(getEmployeeCriteria);

            if (formCollection["command"] == Resources.Labels.Delete)
            {
                var deleteAnnualTransactionCriteria = new DeleteAnnualLeaveTransactionCriteria
                {
                    Id = (int)model.Id,
                    ModifiedDate = DateTime.Now,
                    ModifiedBy = this.CurrentUser.Id
                };
                var result = _employeeService.DeleteAnnualLeaveTransaction(deleteAnnualTransactionCriteria);

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
                var updateAnnualLeaveTransaction = new UpdateAnnualLeaveTransactionCriteria
                {
                    Id = (int)model.Id,
                    Description = model.Description,
                    FromDate = model.AffectedFrom,
                    DeductDays = model.DeductDays,
                    CreditDays = model.CreditDays,
                    ModifiedDate = DateTime.Now,
                    ModifiedBy = this.CurrentUser.Id
                };

                ViewBag.Updated = _employeeService.UpdateAnnualLeaveTransaction(updateAnnualLeaveTransaction);
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
            var getEmployeeCriteria = new GetEmployeeCriteria() { Id = id };
            ViewBag.Employee = _employeeService.GetEmployeeById(getEmployeeCriteria);
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
            var getEmployeeCriteria = new GetEmployeeCriteria() { Id = model.EmpId };
            ViewBag.Employee = _employeeService.GetEmployeeById(getEmployeeCriteria);

            if (ModelState.IsValid)
            {
                var insertAnnualLeaveTransactionCriteria = new InsertAnnualLeaveTransactionCriteria
                {
                    EmployeeId = model.EmpId,
                    Description = model.Description,
                    FromDate = model.AffectedFrom,
                    ToDate = model.AffectedFrom,
                    DeductDays = model.DeductDays,
                    CreditDays = model.CreditDays,
                    CreatedDate = DateTime.Now,
                    CreatedBy = this.CurrentUser.Id
                };

                ViewBag.Inserted = _employeeService.InsertAnnualLeaveTransaction(insertAnnualLeaveTransactionCriteria);
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return View(model);
        }

        /// <summary>
        /// Method to get Model for show and hide open balance or close balance.
        /// </summary>
        /// <returns>Show Open And Close Annual Balance.</returns>
        private ShowOpenAndCloseAnnualBalance GetOpenAndCloseAnnualBalance()
        {
            var model = new ShowOpenAndCloseAnnualBalance();
            var criteria = new GetOpenAndCloseAnnualBalanceCriteria()
            {
                OpenDate = new DateTime(model.ToDate.Year, 1, 1),
                CloseDate = new DateTime(model.FromDate.Year, 12, 31)
            };
            model = _employeeService.GetOpenAndCloseAnnualBalance(criteria);

            return model;
        }
    }
}