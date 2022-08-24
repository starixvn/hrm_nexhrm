namespace Fanex.NexHR.Controllers
{

    using Core.Enums;
    using Criteria.AnnualLeave;
    using Criteria.Employee;
    using Fanex.NexHR.Models.Attributes;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.AnnualLeave;
    using Resources;
    using Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using ViewModels.AnnualLeave;

    public class AnnualLeaveController : BaseController
    {
        private readonly IAnnualLeaveService annualLeaveService;
        private readonly ICommandService commandService;
        private readonly IEmployeeService employeeService;
        private readonly ITeamService teamService;

        public AnnualLeaveController(
            ISessionService sessionService,
            IEmployeeService employeeService,
            ITeamService teamService,
            IAnnualLeaveService annualLeaveService,
            ICommandService commandService) : base(sessionService)
        {
            this.employeeService = employeeService;
            this.teamService = teamService;
            this.annualLeaveService = annualLeaveService;
            this.commandService = commandService;
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public JsonResult AddLeaveConversion(LeaveConversionViewModel model)
        {
            CommandResult result;
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
                result = commandService.ExecuteNonQuery(criteria);
            }
            else
            {
                result = new CommandResult
                {
                    Code = 1,
                    Message = Messages.InvalidInputData
                };
            }

            return Json(result);
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public JsonResult AddLeaveDeduction(LeaveDeductionViewModel model)
        {
            CommandResult result;
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
                result = commandService.ExecuteNonQuery(criteria);
            }
            else
            {
                result = new CommandResult
                {
                    Code = 1,
                    Message = Messages.InvalidInputData
                };
            }

            return Json(result);
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public JsonResult AdjustAnnualLeaves(AddMultiAnnualLeaveTransactionViewModel model)
        {
            CommandResult result = new CommandResult
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
                    DeductDays = model.TotalDays > 0 ? 0 : -model.TotalDays,
                    CreditDays = model.TotalDays > 0 ? model.TotalDays : 0,
                    CreatedBy = CurrentUser.Id
                };

                result = commandService.ExecuteNonQuery(criteria);
            }

            return Json(result);
        }

        [HrmAuthorize(Roles = "HR")]
        public ActionResult Create(int employeeId)
        {
            var getEmployeeCriteria = new GetEmployeeByIdCriteria { EmployeeId = employeeId };
            ViewBag.Employee = employeeService.GetEmployeeById(getEmployeeCriteria);

            return View(new AddAnnualLeaveTransactionViewModel
            {
                EmployeeId = employeeId,
                AffectedFrom = DateTime.Now,
            });
        }

        [HrmAuthorize(Roles = "HR")]
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
                    DeductDays = model.TotalDays > 0 ? 0 : -model.TotalDays,
                    CreditDays = model.TotalDays > 0 ? model.TotalDays : 0,
                    CreatedBy = CurrentUser.Id
                };

                CommandResult result = commandService.ExecuteNonQuery(criteria);
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

        public PartialViewResult CreateLeaveConversion()
        {
            return PartialView(new LeaveConversionViewModel
            {
                OnDate = DateTime.Now,
                TotalDays = decimal.Parse("0.5"),
                Description = Labels.LeaveConversionDescription
            });
        }

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

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public JsonResult Delete(int leaveTransactionId)
        {
            var criteria = new DeleteAnnualLeaveTransactionCriteria
            {
                LeaveTransactionId = leaveTransactionId,
                ModifiedBy = CurrentUser.Id
            };
            CommandResult result = commandService.ExecuteNonQuery(criteria);
            return Json(result);
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public JsonResult DeleteLeaveConversion(int leaveConversionId)
        {
            var criteria = new DeleteLeaveConversionCriteria
            {
                LeaveConversionId = leaveConversionId,
                ModifiedBy = CurrentUser.Id
            };

            return Json(commandService.ExecuteNonQuery(criteria));
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public JsonResult DeleteLeaveDeduction(int leaveDeductionId)
        {
            var criteria = new DeleteLeaveDeductionCriteria
            {
                LeaveDeductionId = leaveDeductionId,
                ModifiedBy = CurrentUser.Id
            };

            return Json(commandService.ExecuteNonQuery(criteria));
        }

        [HrmAuthorize(Roles = "HR")]
        public ActionResult Edit(int leaveTransactionId)
        {
            var criteria = new GetAnnualLeaveByIdCriteria
            {
                LeaveTransactionId = leaveTransactionId
            };

            AnnualLeaveTransaction leaveTransaction = annualLeaveService.GetAnnualLeaveTransactionById(criteria);
            var model = new EditAnnualLeaveTransactionViewModel
            {
                LeaveTransactionId = leaveTransaction.LeaveTransactionId,
                TotalDays = leaveTransaction.TotalDays,
                AffectedFrom = leaveTransaction.AffectedFrom,
                Description = leaveTransaction.Description,
                EmployeeId = leaveTransaction.Employee.Id
            };
            return View(model);
        }

        [HrmAuthorize(Roles = "HR")]
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
                    DeductDays = model.TotalDays > 0 ? 0 : -model.TotalDays,
                    CreditDays = model.TotalDays > 0 ? model.TotalDays : 0,
                    ModifiedBy = CurrentUser.Id
                };
                CommandResult result = commandService.ExecuteNonQuery(criteria);
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

        public PartialViewResult EditLeaveConversion()
        {
            return PartialView(new LeaveConversionViewModel
            {
                OnDate = DateTime.Now,
                TotalDays = decimal.Parse("0.5"),
                Description = Labels.LeaveConversionDescription
            });
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public JsonResult EditLeaveConversion(LeaveConversionViewModel model)
        {
            CommandResult result;
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
                result = commandService.ExecuteNonQuery(criteria);
            }
            else
            {
                result = new CommandResult
                {
                    Code = 1,
                    Message = Messages.InvalidInputData
                };
            }

            return Json(result);
        }

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

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public JsonResult EditLeaveDeduction(LeaveDeductionViewModel model)
        {
            CommandResult result;
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
                result = commandService.ExecuteNonQuery(criteria);
            }
            else
            {
                result = new CommandResult
                {
                    Code = 1,
                    Message = Messages.InvalidInputData
                };
            }

            return Json(result);
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public async Task<ActionResult> Employee(AnnualLeaveBalanceViewModel model)
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

                annualLeaveTransactions = await annualLeaveService.GetAnnualLeaveTransactions(criteria);
            }

            return PartialView("_Employee", annualLeaveTransactions);
        }

        [HrmAuthorize(Roles = "HR")]
        public async Task<ActionResult> Employee(int employeeId, int year)
        {
            var model = new AnnualLeaveBalanceViewModel
            {
                EmployeeId = employeeId,
                Year = year,
                ExcludedRequestTrans = false
            };

            var getEmployeeCriteria = new GetEmployeeByIdCriteria { EmployeeId = employeeId };
            Employee employee = await employeeService.GetEmployeeById(getEmployeeCriteria);

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

        public ActionResult Index()
        {
            var model = new LeaveSummaryViewModel
            {
                Year = DateTime.Now.Year,
                EmployeeId = -1,
                Teams = teamService.GetTeam(),
                ShowOpenAndCloseAnnualBalance = GetOpenAndCloseAnnualBalance()
            };

            return View("Index", model);
        }

        [HttpPost]
        public async Task<ActionResult> Index(LeaveSummaryViewModel model)
        {
            IList<LeaveSummary> leaveSummary = new List<LeaveSummary>();
            IList<AnnualLeaveTransaction> annualLeaveTransactions = new List<AnnualLeaveTransaction>();
            if (model != null)
            {
                var getAnnualLeaveCriteria = new GetAnnualLeaveSummaryCriteria
                {
                    EmployeeId = model.EmployeeId != -1 ? new int?(model.EmployeeId) : null,
                    GroupId = model.TeamId,
                    Year = model.Year,
                    MinAnnualBalance = model.MinAnnualBalance
                };
                leaveSummary = await annualLeaveService.GetAnnualLeaveBalance(getAnnualLeaveCriteria);
                var criteria = new GetAnnualLeaveTransactionByYearCriteria
                {
                    Year = model.Year
                };

                annualLeaveTransactions = await annualLeaveService.GetAnnualLeaveTransactionsByYear(criteria);
                ViewBag.Year = model.Year;
            }

            var leaveSummaryTransactions = new LeaveSummaryTransactions
            {
                leaveSummaries = leaveSummary,
                annualLeaveTransactions = annualLeaveTransactions
            };

            return PartialView("_Index", leaveSummaryTransactions);
        }

        [HttpPost]
        public JsonResult LeaveConversionDetail(LeaveConversionDetailViewModel model)
        {
            IList<LeaveConversionDetail> leaveConversions = new List<LeaveConversionDetail>();
            if (model != null)
            {
                var criteria = new GetLeaveConversionDetailCriteria
                {
                    EmployeeId = model.EmployeeId <= 0 ? null : new int?(model.EmployeeId),
                    GroupId = model.TeamId,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    ConversionType = (int)LeaveConversionType.OvertimeConversion
                };

                leaveConversions = annualLeaveService.GetLeaveConversionDetail(criteria);
            }

            return Json(leaveConversions);
        }

        public async Task<ActionResult> LeaveConversionDetail(
            DateTime fromDate,
            DateTime toDate,
            int? employeeId = null,
            int? teamId = null)
        {
            return View(await GetLeaveDetailModel(fromDate, toDate, employeeId, teamId));
        }

        [HttpPost]
        public JsonResult LeaveDeductionDetail(LeaveConversionDetailViewModel model)
        {
            IList<LeaveConversionDetail> leaveDeductions = new List<LeaveConversionDetail>();
            if (model != null)
            {
                var criteria = new GetLeaveConversionDetailCriteria
                {
                    EmployeeId = model.EmployeeId <= 0 ? null : new int?(model.EmployeeId),
                    GroupId = model.TeamId,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    ConversionType = (int)LeaveConversionType.LateInEarlyOutDeduction
                };

                leaveDeductions = annualLeaveService.GetLeaveConversionDetail(criteria);
            }

            return Json(new { data = leaveDeductions });
        }

        public async Task<ActionResult> LeaveDeductionDetail(
            DateTime fromDate,
            DateTime toDate,
            int? employeeId = null,
            int? teamId = null)
        {
            return View(await GetLeaveDetailModel(fromDate, toDate, employeeId, teamId));
        }

        [HrmAuthorize(Roles = "HR")]
        public PartialViewResult MultipleInsert()
        {
            return PartialView(new AddMultiAnnualLeaveTransactionViewModel
            {
                AffectedFrom = DateTime.Now
            });
        }

        [HrmAuthorize(Roles = "HR")]
        public PartialViewResult MultipleReset()
        {
            return PartialView(new AddMultiAnnualLeaveTransactionViewModel
            {
                AffectedFrom = DateTime.Now
            });
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public JsonResult OpenAnnualLeaves()
        {
            var criteria = new OpenAnnualBalanceForNewYearCriteria
            {
                Year = ShowOpenAndCloseAnnualBalance.ToDate.Year
            };

            return Json(commandService.ExecuteNonQuery(criteria));
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public async Task<JsonResult> ResetAnnualLeaves(int minbalance, string descriptionReset)
        {
            var getAnnualLeaveCriteria = new GetAnnualLeaveSummaryCriteria
            {
                EmployeeId = null,
                GroupId = null,
                Year = ShowOpenAndCloseAnnualBalance.FromDate.Year
            };

            IList<LeaveSummary> leaveSummaries = await annualLeaveService.GetAnnualLeaveBalance(getAnnualLeaveCriteria);
            var leaveSummariesFilter = leaveSummaries.Where(m => m.LeaveBalance > minbalance).ToList();

            var resetAnnualLeaves = new ResetAnnualLeavesCriteria
            {
                Description = descriptionReset,
                FromDate = new DateTime(ShowOpenAndCloseAnnualBalance.FromDate.Year, 12, 31),
                ResetDays = minbalance,
                CreatedBy = CurrentUser.Id,
                LeaveSummaries = leaveSummariesFilter
            };

            CommandResult result = commandService.ExecuteNonQuery(resetAnnualLeaves);

            return Json(result);
        }

        private async Task<LeaveConversionDetailViewModel> GetLeaveDetailModel(DateTime fromDate, DateTime toDate, int? employeeId, int? teamId)
        {
            IList<Team> teams = teamService.GetTeam();

            var model = new LeaveConversionDetailViewModel
            {
                EmployeeId = employeeId ?? -1,
                FromDate = fromDate,
                ToDate = toDate,
                TeamId = teamId,
                Teams = teams
            };

            if (employeeId.HasValue)
            {
                var getEmployeeCriteria = new GetEmployeeByIdCriteria { EmployeeId = employeeId.Value };
                Employee employee = await employeeService.GetEmployeeById(getEmployeeCriteria);

                model.Employee = employee.DisplayName;
            }

            return model;
        }

        private ShowOpenAndCloseAnnualBalance GetOpenAndCloseAnnualBalance()
        {
            var criteria = new GetOpenAndCloseAnnualBalanceCriteria
            {
                OpenDate = new DateTime(ShowOpenAndCloseAnnualBalance.ToDate.Year, 1, 1),
                CloseDate = new DateTime(ShowOpenAndCloseAnnualBalance.FromDate.Year, 12, 31)
            };

            ShowOpenAndCloseAnnualBalance model = annualLeaveService.GetOpenAndCloseAnnualBalance(criteria);

            return model;
        }
    }
}