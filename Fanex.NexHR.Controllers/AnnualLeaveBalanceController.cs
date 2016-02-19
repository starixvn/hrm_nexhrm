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
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Criteria.Report;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;

    [Authorize(Roles = "HR")]
    public class AnnualLeaveBalanceController : BaseController
    {
        private readonly IEmployeeService _employeeService;
        private readonly ITeamService _teamService;
        private readonly IReportRepository _reportRepository;

        public AnnualLeaveBalanceController()
            : this(null, null, null)
        {
        }

        public AnnualLeaveBalanceController(
            IEmployeeService employeeService,
            ITeamService teamService,
            IReportRepository reportRepository)
        {
            _employeeService = employeeService ?? new EmployeeService();
            _teamService = teamService ?? new TeamService();
            _reportRepository = reportRepository ?? new ReportRepository();
        }

        public ActionResult Index()
        {
            IList<Team> teams = _teamService.GetAll(new GetTeamCriteria());

            var model = new LeaveSummaryViewModel
            {
                Year = DateTime.Now.Year,
                EmployeeId = -1,
                Teams = teams,
                ShowOpenAndCloseAnnualBalance = GetOpenAndCloseAnnualBalance()
            };

            return View("Index", model);
        }

        [HttpPost]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed. Suppression is OK here.")]
        public ActionResult Index(LeaveSummaryViewModel model)
        {
            IList<Team> allTeam = _teamService.GetAll(new GetTeamCriteria());
            if (model == null)
            {
                model = new LeaveSummaryViewModel
                {
                    Year = DateTime.Now.Year,
                    EmployeeId = -1
                };
            }

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

            var getAnnualLeaveCriteria = new GetAnnualLeaveSummaryReportCriteria
            {
                EmployeeId = model.EmployeeId != -1 ? new int?(model.EmployeeId) : null,
                GroupId = model.TeamId,
                Year = model.Year
            };
            IList<LeaveSummary> entityList = _reportRepository.GetAnnualLeaveBalanceReport(getAnnualLeaveCriteria);

            IList<LeaveSummaryByTeam> leaveSummaryByTeamList = _employeeService.GetAnnualLeaveByTeam(entityList, listTeam);
            if (model.MinAnnualBalance > 0)
            {
                foreach (LeaveSummaryByTeam item in leaveSummaryByTeamList)
                {
                    item.LeaveSummaryInTeam = item.LeaveSummaryInTeam.Where(m => m.LeaveBalance >= model.MinAnnualBalance).ToList();
                }
            }

            ViewBag.Year = model.Year;

            return PartialView("_Index", leaveSummaryByTeamList);
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

        public ActionResult Employee(int id = -1)
        {
            var model = new AnnualLeaveBalanceViewModel
            {
                EmployeeId = id,
                Year = DateTime.Now.Year,
                ExcludedRequestTrans = false
            };

            var getEmployeeCriteria = new GetEmployeeByIdCriteria() { EmployeeId = id };
            Employee employee = _employeeService.GetEmployeeById(getEmployeeCriteria);

            if (employee != null)
            {
                ViewBag.Employee = employee;
            }

            return View("Employee", model);
        }

        [HttpPost]
        public ActionResult Employee(AnnualLeaveBalanceViewModel model)
        {
            var getEmployeeCriteria = new GetEmployeeByIdCriteria() { EmployeeId = model.EmployeeId };
            ViewBag.Employee = _employeeService.GetEmployeeById(getEmployeeCriteria);

            var getAnnualLeaveTransactionCriteria = new GetAnnualLeaveTransactionCriteria
            {
                EmployeeId = model.EmployeeId,
                Year = model.Year,
                ExcludedRequestTransactions = model.ExcludedRequestTrans
            };

            IList<AnnualLeaveBalanceModel> annualLeaveTransactions = _employeeService.GetAnnualLeaveTransactions(getAnnualLeaveTransactionCriteria);

            return PartialView("_Employee", annualLeaveTransactions);
        }

        public ActionResult Edit(int id = -1)
        {
            var getAnnualLeaveByIdCriteria = new GetAnnualLeaveByIdCriteria
            {
                Id = id
            };

            AnnualLeaveBalanceModel model = _employeeService.GetAnnualLeaveTransactionById(getAnnualLeaveByIdCriteria);

            if (model != null)
            {
                var getEmployeeCriteria = new GetEmployeeByIdCriteria() { EmployeeId = model.EmpId };
                ViewBag.Employee = _employeeService.GetEmployeeById(getEmployeeCriteria);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(FormCollection formCollection, AnnualLeaveBalanceModel model)
        {
            var getEmployeeCriteria = new GetEmployeeByIdCriteria() { EmployeeId = model.EmpId };
            ViewBag.Employee = _employeeService.GetEmployeeById(getEmployeeCriteria);

            if (formCollection["command"] == Labels.Delete)
            {
                var deleteAnnualTransactionCriteria = new DeleteAnnualLeaveTransactionCriteria
                {
                    Id = (int)model.Id,
                    ModifiedDate = DateTime.Now,
                    ModifiedBy = this.CurrentUser.Id
                };

                bool result = _employeeService.DeleteAnnualLeaveTransaction(deleteAnnualTransactionCriteria);

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

        public ActionResult Create(int id = -1)
        {
            var getEmployeeCriteria = new GetEmployeeByIdCriteria() { EmployeeId = id };
            ViewBag.Employee = _employeeService.GetEmployeeById(getEmployeeCriteria);

            return View(new AnnualLeaveBalanceModel
            {
                EmpId = id,
                AffectedFrom = DateTime.Now
            });
        }

        [HttpPost]
        public ActionResult Create(AnnualLeaveBalanceModel model)
        {
            var getEmployeeCriteria = new GetEmployeeByIdCriteria() { EmployeeId = model.EmpId };
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