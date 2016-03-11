namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.AnnualLeave;
    using Fanex.NexHR.Criteria.Employee;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.AnnualLeave;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.AnnualLeave;

    public class AnnualLeaveController : BaseController
    {
        private readonly IEmployeeService employeeService;
        private readonly ITeamService teamService;
        private readonly IAnnualLeaveService annualLeaveService;
        private readonly ICommandService commandService;

        public AnnualLeaveController()
            : this(null, null, null, null)
        {
        }

        public AnnualLeaveController(
            IEmployeeService employeeService,
            ITeamService teamService,
            IAnnualLeaveService annualLeaveService,
            ICommandService commandService)
        {
            this.employeeService = employeeService ?? new EmployeeService();
            this.teamService = teamService ?? new TeamService();
            this.annualLeaveService = annualLeaveService ?? new AnnualLeaveService();
            this.commandService = commandService ?? new CommandService();
        }

        public ActionResult Index()
        {
            var model = new LeaveSummaryViewModel
            {
                Year = DateTime.Now.Year,
                EmployeeId = -1,
                Teams = teamService.GetAll(),
                ShowOpenAndCloseAnnualBalance = GetOpenAndCloseAnnualBalance()
            };

            return View("Index", model);
        }

        [HttpPost]
        public ActionResult Index(LeaveSummaryViewModel model)
        {
            IList<LeaveSummary> leaveSummary = new List<LeaveSummary>();
            if (model != null)
            {
                var getAnnualLeaveCriteria = new GetAnnualLeaveSummaryCriteria
                {
                    EmployeeId = model.EmployeeId != -1 ? new int?(model.EmployeeId) : null,
                    GroupId = model.TeamId,
                    Year = model.Year,
                    MinAnnualBalance = model.MinAnnualBalance
                };
                leaveSummary = annualLeaveService.GetAnnualLeaveBalance(getAnnualLeaveCriteria);
                ViewBag.Year = model.Year;
            }

            return PartialView("_Index", leaveSummary);
        }

        [Authorize(Roles = "HR")]
        [ChildActionOnly]
        public PartialViewResult MultipleInsert()
        {
            return PartialView(new AddMultiAnnualLeaveTransactionViewModel
            {
                AffectedFrom = DateTime.Now
            });
        }

        [Authorize(Roles = "HR")]
        [ChildActionOnly]
        public PartialViewResult MultipleReset()
        {
            return PartialView(new AddMultiAnnualLeaveTransactionViewModel
            {
                AffectedFrom = DateTime.Now
            });
        }

        [Authorize(Roles = "HR")]
        public ActionResult Employee(int employeeId, int year)
        {
            var model = new AnnualLeaveBalanceViewModel
            {
                EmployeeId = employeeId,
                Year = year,
                ExcludedRequestTrans = false
            };

            var getEmployeeCriteria = new GetEmployeeByIdCriteria() { EmployeeId = employeeId };
            Employee employee = employeeService.GetEmployeeById(getEmployeeCriteria);

            if (employee == null || employee.Id == 0)
            {
                return RedirectToAction("NoData", "Home");
            }
            else
            {
                model.EmployeeNickName = employee.NickName;
            }

            return View("Employee", model);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public ActionResult Employee(AnnualLeaveBalanceViewModel model)
        {
            IList<AnnualLeaveTransaction> annualLeaveTransactions = new List<AnnualLeaveTransaction>();
            if (model != null)
            {
                var criteria = new GetAnnualLeaveTransactionCriteria
                {
                    EmployeeId = model.EmployeeId,
                    Year = model.Year,
                    ExcludedRequestTransactions = model.ExcludedRequestTrans
                };

                annualLeaveTransactions = annualLeaveService.GetAnnualLeaveTransactions(criteria);
            }

            return PartialView("_Employee", annualLeaveTransactions);
        }

        [Authorize(Roles = "HR")]
        public ActionResult Create(int employeeId)
        {
            var getEmployeeCriteria = new GetEmployeeByIdCriteria() { EmployeeId = employeeId };
            ViewBag.Employee = employeeService.GetEmployeeById(getEmployeeCriteria);

            return View(new AddAnnualLeaveTransactionViewModel
            {
                EmployeeId = employeeId,
                AffectedFrom = DateTime.Now,
            });
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public ActionResult Create(AddAnnualLeaveTransactionViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var criteria = new InsertAnnualLeaveTransactionCriteria
                {
                    EmployeeId = model.EmployeeId,
                    Description = model.Description,
                    FromDate = model.AffectedFrom,
                    ToDate = model.AffectedFrom,
                    DeductDays = model.TotalDays > 0 ? 0 : model.TotalDays,
                    CreditDays = model.TotalDays > 0 ? model.TotalDays : 0,
                    CreatedBy = CurrentUser.Id
                };

                CommandResult result = commandService.Insert(criteria);
                if (result.IsSuccessful)
                {
                    ViewBag.Inserted = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult Edit(int leaveTransactionId)
        {
            var criteria = new GetAnnualLeaveByIdCriteria
            {
                LeaveTransactionId = leaveTransactionId
            };

            AnnualLeaveTransaction leaveTransaction = annualLeaveService.GetAnnualLeaveTransactionById(criteria);
            var model = new EditAnnualLeaveTransactionViewModel()
            {
                LeaveTransactionId = leaveTransaction.LeaveTransactionId,
                TotalDays = leaveTransaction.TotalDays,
                AffectedFrom = leaveTransaction.AffectedFrom,
                Description = leaveTransaction.Description,
                EmployeeId = leaveTransaction.Employee.Id
            };
            return View(model);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public ActionResult Edit(EditAnnualLeaveTransactionViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var criteria = new UpdateAnnualLeaveTransactionCriteria
                {
                    LeaveTransactionId = model.LeaveTransactionId,
                    Description = model.Description,
                    FromDate = model.AffectedFrom,
                    DeductDays = model.TotalDays > 0 ? 0 : model.TotalDays,
                    CreditDays = model.TotalDays > 0 ? model.TotalDays : 0,
                    ModifiedBy = CurrentUser.Id
                };
                CommandResult result = commandService.Update(criteria);
                if (result.IsSuccessful)
                {
                    ViewBag.Updated = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            return View(model);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult Delete(int leaveTransactionId)
        {
            var criteria = new DeleteAnnualLeaveTransactionCriteria
            {
                LeaveTransactionId = leaveTransactionId,
                ModifiedBy = CurrentUser.Id
            };
            CommandResult result = commandService.Delete(criteria);
            return Json(result);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult AdjustAnnualLeaves(AddMultiAnnualLeaveTransactionViewModel model)
        {
            CommandResult result = new CommandResult()
            {
                Code = -1,
                Message = Messages.InvalidInputData
            };

            if (model != null && ModelState.IsValid)
            {
                var criteria = new AdjustAnnualLeavesCriteria
                {
                    EmployeeId = model.EmpIds,
                    Description = model.Description,
                    FromDate = model.AffectedFrom,
                    DeductDays = model.TotalDays > 0 ? 0 : model.TotalDays,
                    CreditDays = model.TotalDays > 0 ? model.TotalDays : 0,
                    CreatedBy = CurrentUser.Id
                };

                result = commandService.Insert(criteria);
            }

            return Json(result);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult OpenAnnualLeaves()
        {
            var model = new ShowOpenAndCloseAnnualBalance();
            var criteria = new OpenAnnualBalanceForNewYearCriteria()
            {
                Year = model.ToDate.Year
            };

            return Json(commandService.Insert(criteria));
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult ResetAnnualLeaves(int minbalance, string descriptionReset)
        {
            var model = new ShowOpenAndCloseAnnualBalance();
            var getAnnualLeaveCriteria = new GetAnnualLeaveSummaryCriteria
            {
                EmployeeId = null,
                GroupId = null,
                Year = model.FromDate.Year
            };

            IList<LeaveSummary> leaveSummaries = annualLeaveService.GetAnnualLeaveBalance(getAnnualLeaveCriteria).Where(m => m.LeaveBalance > minbalance).ToList();

            var resetAnnualLeaves = new ResetAnnualLeavesCriteria
            {
                Description = descriptionReset,
                FromDate = new DateTime(model.FromDate.Year, 12, 31),
                ResetDays = minbalance,
                CreatedBy = CurrentUser.Id,
                LeaveSummaries = leaveSummaries
            };

            CommandResult result = commandService.Update(resetAnnualLeaves);

            return Json(result);
        }

        [ChildActionOnly]
        public PartialViewResult CreateLeaveDeduction()
        {
            return PartialView(new LeaveDeductionViewModel
            {
                DeductDate = DateTime.Now,
                TotalDays = decimal.Parse("0.5"),
                Description = Labels.LeaveDeductionDescription,
                IsAnnual = true
            });
        }

        [ChildActionOnly]
        public PartialViewResult EditLeaveDeduction()
        {
            return PartialView(new LeaveDeductionViewModel
            {
                DeductDate = DateTime.Now,
                TotalDays = decimal.Parse("0.5"),
                Description = Labels.LeaveDeductionDescription,
                IsAnnual = true
            });
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult AddLeaveDeduction(LeaveDeductionViewModel model)
        {
            CommandResult result = new CommandResult();
            if (model != null && ModelState.IsValid)
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
                result = commandService.Insert(criteria);
            }
            else
            {
                result = new CommandResult()
                {
                    Code = 1,
                    Message = Messages.InvalidInputData
                };
            }

            return Json(result);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult EditLeaveDeduction(LeaveDeductionViewModel model)
        {
            CommandResult result = new CommandResult();
            if (model != null && ModelState.IsValid)
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
                result = commandService.Update(criteria);
            }
            else
            {
                result = new CommandResult()
                {
                    Code = 1,
                    Message = Messages.InvalidInputData
                };
            }

            return Json(result);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult DeleteLeaveDeduction(int leaveDeductionId)
        {
            var criteria = new DeleteLeaveDeductionCriteria
            {
                LeaveDeductionId = leaveDeductionId,
                ModifiedBy = CurrentUser.Id
            };

            return Json(commandService.Delete(criteria));
        }

        public ActionResult LeaveDeductionDetail(
            DateTime fromDate,
            DateTime toDate,
            int? employeeId = null,
            int? teamId = null)
        {
            IList<Team> teams = teamService.GetAll();

            var model = new LeaveConversionDetailViewModel
            {
                EmployeeId = employeeId.HasValue ? employeeId.Value : -1,
                FromDate = fromDate,
                ToDate = toDate,
                TeamId = teamId,
                Teams = teams
            };

            if (employeeId.HasValue)
            {
                var getEmployeeCriteria = new GetEmployeeByIdCriteria() { EmployeeId = employeeId.Value };
                Employee employee = employeeService.GetEmployeeById(getEmployeeCriteria);

                model.Employee = employee.DisplayName;
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult LeaveDeductionDetail(LeaveConversionDetailViewModel model)
        {
            IList<LeaveConversionDetail> leaveDeductions = new List<LeaveConversionDetail>();
            if (model != null)
            {
                var criteria = new GetLeaveConversionDetailCriteria()
                {
                    EmployeeId = model.EmployeeId <= 0 ? null : new int?(model.EmployeeId),
                    GroupId = model.TeamId,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    ConversionType = (int)LeaveConversionType.LateInEarlyOutDeduction
                };

                leaveDeductions = annualLeaveService.GetLeaveConversionDetail(criteria);
            }

            return Json(new { data = leaveDeductions }, JsonRequestBehavior.AllowGet);
        }

        [ChildActionOnly]
        public PartialViewResult CreateLeaveConversion()
        {
            return PartialView(new LeaveConversionViewModel
            {
                OnDate = DateTime.Now,
                TotalDays = decimal.Parse("0.5"),
                Description = Labels.LeaveConversionDescription
            });
        }

        [ChildActionOnly]
        public PartialViewResult EditLeaveConversion()
        {
            return PartialView(new LeaveConversionViewModel
            {
                OnDate = DateTime.Now,
                TotalDays = decimal.Parse("0.5"),
                Description = Labels.LeaveConversionDescription
            });
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult AddLeaveConversion(LeaveConversionViewModel model)
        {
            CommandResult result = new CommandResult();
            if (model != null && ModelState.IsValid)
            {
                var criteria = new InsertLeaveConversionCriteria
                {
                    EmployeeId = model.EmpId,
                    Description = model.Description,
                    Date = model.OnDate,
                    TotalDays = model.TotalDays,
                    CreatedBy = CurrentUser.Id
                };
                result = commandService.Insert(criteria);
            }
            else
            {
                result = new CommandResult()
                {
                    Code = 1,
                    Message = Messages.InvalidInputData
                };
            }

            return Json(result);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult EditLeaveConversion(LeaveConversionViewModel model)
        {
            CommandResult result = new CommandResult();
            if (model != null && ModelState.IsValid)
            {
                var criteria = new UpdateLeaveConversionCriteria
                {
                    LeaveConversionId = model.LeaveConversionId,
                    Description = model.Description,
                    Date = model.OnDate,
                    TotalDays = model.TotalDays,
                    ModifiedBy = CurrentUser.Id
                };
                result = commandService.Update(criteria);
            }
            else
            {
                result = new CommandResult()
                {
                    Code = 1,
                    Message = Messages.InvalidInputData
                };
            }

            return Json(result);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult DeleteLeaveConversion(int leaveConversionId)
        {
            var criteria = new DeleteLeaveConversionCriteria
            {
                LeaveConversionId = leaveConversionId,
                ModifiedBy = CurrentUser.Id
            };

            return Json(commandService.Delete(criteria));
        }

        public ActionResult LeaveConversionDetail(
            DateTime fromDate,
            DateTime toDate,
            int? employeeId = null,
            int? teamId = null)
        {
            IList<Team> teams = teamService.GetAll();

            var model = new LeaveConversionDetailViewModel
            {
                EmployeeId = employeeId.HasValue ? employeeId.Value : -1,
                FromDate = fromDate,
                ToDate = toDate,
                TeamId = teamId,
                Teams = teams
            };

            if (employeeId.HasValue)
            {
                var getEmployeeCriteria = new GetEmployeeByIdCriteria() { EmployeeId = employeeId.Value };
                Employee employee = employeeService.GetEmployeeById(getEmployeeCriteria);

                model.Employee = employee.DisplayName;
            }

            return View(model);
        }

        [HttpPost]
        public JsonResult LeaveConversionDetail(LeaveConversionDetailViewModel model)
        {
            IList<LeaveConversionDetail> leaveConversions = new List<LeaveConversionDetail>();
            if (model != null)
            {
                var criteria = new GetLeaveConversionDetailCriteria()
                {
                    EmployeeId = model.EmployeeId <= 0 ? null : new int?(model.EmployeeId),
                    GroupId = model.TeamId,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    ConversionType = (int)LeaveConversionType.OvertimeConversion
                };

                leaveConversions = annualLeaveService.GetLeaveConversionDetail(criteria);
            }

            return Json(leaveConversions, JsonRequestBehavior.AllowGet);
        }

        private ShowOpenAndCloseAnnualBalance GetOpenAndCloseAnnualBalance()
        {
            var model = new ShowOpenAndCloseAnnualBalance();
            var criteria = new GetOpenAndCloseAnnualBalanceCriteria()
            {
                OpenDate = new DateTime(model.ToDate.Year, 1, 1),
                CloseDate = new DateTime(model.FromDate.Year, 12, 31)
            };

            model = annualLeaveService.GetOpenAndCloseAnnualBalance(criteria);

            return model;
        }
    }
}