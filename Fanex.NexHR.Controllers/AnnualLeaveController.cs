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
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;
    using Fanex.NexHR.ViewModels.AnnualLeave;

    [Authorize(Roles = "HR")]
    public class AnnualLeaveController : BaseController
    {
        private readonly IEmployeeService _employeeService;
        private readonly ITeamService _teamService;
        private readonly IReportService _reportService;

        public AnnualLeaveController()
            : this(null, null, null)
        {
        }

        public AnnualLeaveController(
            IEmployeeService employeeService,
            ITeamService teamService,
            IReportService reportService)
        {
            _employeeService = employeeService ?? new EmployeeService();
            _teamService = teamService ?? new TeamService();
            _reportService = reportService ?? new ReportService();
        }

        public ActionResult Index()
        {
            var model = new LeaveSummaryViewModel
            {
                Year = DateTime.Now.Year,
                EmployeeId = -1,
                Teams = _teamService.GetAll(),
                ShowOpenAndCloseAnnualBalance = GetOpenAndCloseAnnualBalance()
            };

            return View("Index", model);
        }

        [HttpPost]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Reviewed. Suppression is OK here.")]
        public ActionResult Index(LeaveSummaryViewModel model)
        {
            IList<Team> allTeam = _teamService.GetAll();
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
            IList<LeaveSummary> entityList = _reportService.GetAnnualLeaveBalanceReport(getAnnualLeaveCriteria);

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
                    ModifiedBy = CurrentUser.Id
                };
                CommandResult result = _employeeService.DeleteAnnualLeaveTransaction(deleteAnnualTransactionCriteria);
                if (result.Code == 0)
                {
                    return RedirectToAction("Employee", "AnnualLeaveBalance", new { id = model.EmpId });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                    return View(model);
                }
            }
            else if (ModelState.IsValid)
            {
                var updateAnnualLeaveTransaction = new UpdateAnnualLeaveTransactionCriteria
                {
                    Id = (int)model.Id,
                    Description = model.Description,
                    FromDate = model.AffectedFrom,
                    DeductDays = model.DeductDays,
                    CreditDays = model.CreditDays,
                    ModifiedBy = CurrentUser.Id
                };
                CommandResult result = _employeeService.UpdateAnnualLeaveTransaction(updateAnnualLeaveTransaction);
                if (result.Code == 0)
                {
                    ViewBag.Updated = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Messages.InvalidInputData);
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

                CommandResult result = _employeeService.InsertAnnualLeaveTransaction(insertAnnualLeaveTransactionCriteria);
                if (result.Code == 0)
                {
                    ViewBag.Inserted = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Messages.InvalidInputData);
            }

            return View(model);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult AdjustAnnualLeaves(AnnualLeaveBalanceMultiViewModel model)
        {
            if (ModelState.IsValid)
            {
                var adjustAnnualLeavesCriteria = new AdjustAnnualLeavesCriteria
                {
                    EmployeeId = model.EmpIds,
                    Description = model.Description,
                    FromDate = model.AffectedFrom,
                    DeductDays = model.DeductDays,
                    CreditDays = model.CreditDays,
                    CreatedDate = DateTime.Now,
                    CreatedBy = this.CurrentUser.Id
                };

                return Json(_employeeService.AdjustAnnualLeaves(adjustAnnualLeavesCriteria));
            }
            else
            {
                return Json(-1);
            }
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult OpenAnnualLeaves()
        {
            var model = new ShowOpenAndCloseAnnualBalance();
            var criteria = new OpenAnnualBalanceForNewYearCriteria()
            {
                Year = model.ToDate.Year,
                CreatedDate = DateTime.Now.Date
            };

            return Json(_employeeService.OpenAnnualLeaves(criteria));
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult ResetAnnualLeaves(int minbalance, string descriptionReset)
        {
            if (ModelState.IsValid)
            {
                var model = new ShowOpenAndCloseAnnualBalance();

                var getAnnualLeaveCriteria = new GetAnnualLeaveSummaryReportCriteria
                {
                    EmployeeId = null,
                    GroupId = null,
                    Year = model.FromDate.Year
                };

                IList<LeaveSummary> entityList = _reportService.GetAnnualLeaveBalanceReport(getAnnualLeaveCriteria).Where(m => m.LeaveBalance > minbalance).ToList();

                var resetAnnualLeaves = new ResetAnnualLeavesCriteria
                {
                    Description = descriptionReset,
                    FromDate = new DateTime(model.FromDate.Year, 12, 31),
                    ResetDays = minbalance,
                    CreatedDate = DateTime.Now,
                    CreatedBy = this.CurrentUser.Id,
                    LeaveSummaries = entityList
                };

                return Json(_employeeService.ResetAnnualLeaves(resetAnnualLeaves));
            }
            else
            {
                return Json(-1);
            }
        }

        [HttpPost]
        public JsonResult AddLeaveDeduction(LeaveDeductionViewModel model)
        {
            var criteria = new InsertLeaveDeductionCriteria
            {
                EmployeeId = model.EmpId,
                Description = model.Description,
                DeductDate = model.DeductDate,
                IsAnnual = model.IsAnnual,
                TotalDays = model.TotalDays,
                CreatedBy = CurrentUser.Id
            };

            return Json(_employeeService.AddLeaveDeduction(criteria));
        }

        [HttpPost]
        public JsonResult EditLeaveDeduction(LeaveDeductionViewModel model)
        {
            var criteria = new UpdateLeaveDeductionCriteria
            {
                LeaveDeductionId = model.LeaveDeductionId,
                Description = model.Description,
                DeductDate = model.DeductDate,
                IsAnnual = model.IsAnnual,
                TotalDays = model.TotalDays,
                ModifiedBy = CurrentUser.Id
            };

            return Json(_employeeService.EditLeaveDeduction(criteria));
        }

        [HttpPost]
        public JsonResult DeleteLeaveDeduction(int leaveDeductionId)
        {
            var criteria = new DeleteLeaveDeductionCriteria
            {
                LeaveDeductionId = leaveDeductionId,
                ModifiedBy = CurrentUser.Id
            };

            return Json(_employeeService.DeleteLeaveDeduction(criteria));
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