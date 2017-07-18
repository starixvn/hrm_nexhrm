namespace Fanex.NexHR.Controllers
{
    using Core.Enums;
    using Criteria.AnnualLeave;
    using Criteria.Employee;
    using Models;
    using Models.AnnualLeave;
    using Resources;
    using Services;
    using Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using ViewModels.AnnualLeave;

    public class AnnualLeaveController : BaseController
    {
        private readonly IAnnualLeaveService _annualLeaveService;
        private readonly ICommandService _commandService;
        private readonly IEmployeeService _employeeService;
        private readonly ITeamService _teamService;

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
            _employeeService = employeeService ?? new EmployeeService();
            _teamService = teamService ?? new TeamService();
            _annualLeaveService = annualLeaveService ?? new AnnualLeaveService();
            _commandService = commandService ?? new CommandService();
        }

        [Authorize(Roles = "HR")]
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
                result = _commandService.Insert(criteria);
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

        [Authorize(Roles = "HR")]
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
                result = _commandService.Insert(criteria);
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

        [Authorize(Roles = "HR")]
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

                result = _commandService.Insert(criteria);
            }

            return Json(result);
        }

        [Authorize(Roles = "HR")]
        public ActionResult Create(int employeeId)
        {
            var getEmployeeCriteria = new GetEmployeeByIdCriteria { EmployeeId = employeeId };
            ViewBag.Employee = _employeeService.GetEmployeeById(getEmployeeCriteria);

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
                    DeductDays = model.TotalDays > 0 ? 0 : -model.TotalDays,
                    CreditDays = model.TotalDays > 0 ? model.TotalDays : 0,
                    CreatedBy = CurrentUser.Id
                };

                CommandResult result = _commandService.Insert(criteria);
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

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult Delete(int leaveTransactionId)
        {
            var criteria = new DeleteAnnualLeaveTransactionCriteria
            {
                LeaveTransactionId = leaveTransactionId,
                ModifiedBy = CurrentUser.Id
            };
            CommandResult result = _commandService.Delete(criteria);
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

            return Json(_commandService.Delete(criteria));
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

            return Json(_commandService.Delete(criteria));
        }

        [Authorize(Roles = "HR")]
        public ActionResult Edit(int leaveTransactionId)
        {
            var criteria = new GetAnnualLeaveByIdCriteria
            {
                LeaveTransactionId = leaveTransactionId
            };

            AnnualLeaveTransaction leaveTransaction = _annualLeaveService.GetAnnualLeaveTransactionById(criteria);
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
                    DeductDays = model.TotalDays > 0 ? 0 : -model.TotalDays,
                    CreditDays = model.TotalDays > 0 ? model.TotalDays : 0,
                    ModifiedBy = CurrentUser.Id
                };
                CommandResult result = _commandService.Update(criteria);
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
                result = _commandService.Update(criteria);
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
                result = _commandService.Update(criteria);
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

                annualLeaveTransactions = _annualLeaveService.GetAnnualLeaveTransactions(criteria);
            }

            return PartialView("_Employee", annualLeaveTransactions);
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

            var getEmployeeCriteria = new GetEmployeeByIdCriteria { EmployeeId = employeeId };
            Employee employee = _employeeService.GetEmployeeById(getEmployeeCriteria);

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
                Teams = _teamService.GetTeam(),
                ShowOpenAndCloseAnnualBalance = GetOpenAndCloseAnnualBalance()
            };

            return View("Index", model);
        }

        [ValidateInput(false)]
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
                leaveSummary = _annualLeaveService.GetAnnualLeaveBalance(getAnnualLeaveCriteria);
                ViewBag.Year = model.Year;
            }

            return PartialView("_Index", leaveSummary);
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

                leaveConversions = _annualLeaveService.GetLeaveConversionDetail(criteria);
            }

            return Json(leaveConversions, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LeaveConversionDetail(
            DateTime fromDate,
            DateTime toDate,
            int? employeeId = null,
            int? teamId = null)
        {
            var model = GetLeaveDetailModel(fromDate, toDate, employeeId, teamId);
            return View(model);
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

                leaveDeductions = _annualLeaveService.GetLeaveConversionDetail(criteria);
            }

            return Json(new { data = leaveDeductions }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LeaveDeductionDetail(
            DateTime fromDate,
            DateTime toDate,
            int? employeeId = null,
            int? teamId = null)
        {
            var model = GetLeaveDetailModel(fromDate, toDate, employeeId, teamId);
            return View(model);
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
        [HttpPost]
        public JsonResult OpenAnnualLeaves()
        {
            var criteria = new OpenAnnualBalanceForNewYearCriteria
            {
                Year = ShowOpenAndCloseAnnualBalance.ToDate.Year
            };

            return Json(_commandService.Insert(criteria));
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult ResetAnnualLeaves(int minbalance, string descriptionReset)
        {
            var getAnnualLeaveCriteria = new GetAnnualLeaveSummaryCriteria
            {
                EmployeeId = null,
                GroupId = null,
                Year = ShowOpenAndCloseAnnualBalance.FromDate.Year
            };

            IList<LeaveSummary> leaveSummaries = _annualLeaveService.GetAnnualLeaveBalance(getAnnualLeaveCriteria).Where(m => m.LeaveBalance > minbalance).ToList();

            var resetAnnualLeaves = new ResetAnnualLeavesCriteria
            {
                Description = descriptionReset,
                FromDate = new DateTime(ShowOpenAndCloseAnnualBalance.FromDate.Year, 12, 31),
                ResetDays = minbalance,
                CreatedBy = CurrentUser.Id,
                LeaveSummaries = leaveSummaries
            };

            CommandResult result = _commandService.Update(resetAnnualLeaves);

            return Json(result);
        }

        private LeaveConversionDetailViewModel GetLeaveDetailModel(DateTime fromDate, DateTime toDate, int? employeeId, int? teamId)
        {
            IList<Team> teams = _teamService.GetTeam();

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
                Employee employee = _employeeService.GetEmployeeById(getEmployeeCriteria);

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

            ShowOpenAndCloseAnnualBalance model = _annualLeaveService.GetOpenAndCloseAnnualBalance(criteria);

            return model;
        }
    }
}