namespace Fanex.NexHR.Controllers
{
    using Core.Enums;
    using Criteria.AnnualLeave;
    using Criteria.Employee;
    using Fanex.NexHR.Controllers.Attributes;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.AnnualLeave;
    using Fanex.NexHR.Core;
    using Resources;
    using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ViewModels.AnnualLeave;
using Fanex.NexHR.Services.ExportExcel;

    public class AnnualLeaveController : BaseController
    {
        private readonly IBaseService baseService;
        private readonly IEmployeeService employeeService;
        private readonly ITeamService teamService;
        private readonly IDepartmentService departmentService;

        public AnnualLeaveController(
            ISessionService sessionService,
            IEmployeeService employeeService,
            ITeamService teamService,
            IDepartmentService departmentService,
            IBaseService baseService) : base(sessionService)
        {
            this.employeeService = employeeService;
            this.teamService = teamService;
            this.departmentService = departmentService;
            this.baseService = baseService;
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> AddLeaveConversion(LeaveConversionViewModel model)
        {
            CommandResult result;
            if (model != null && ModelState.IsValid)
            {
                var criteria = new InsertLeaveConversionCommand
                {
                    EmployeeId = model.EmpId,
                    Description = model.Description,
                    Date = model.OnDate,
                    TotalDays = model.TotalDays,
                    CreatedBy = CurrentUser.Id
                };

                result = await baseService.ExecuteNonQueryAsync(criteria);
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

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> AddLeaveDeduction(LeaveDeductionViewModel model)
        {
            CommandResult result;
            if (model != null && ModelState.IsValid)
            {
                var criteria = new InsertLeaveDeductionCommand
                {
                    EmployeeId = model.EmpId,
                    Description = model.Description,
                    DeductDate = model.DeductDate,
                    IsAnnual = model.IsAnnual,
                    TotalDays = model.TotalDays,
                    CreatedBy = CurrentUser.Id
                };
                result = await baseService.ExecuteNonQueryAsync(criteria);
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

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> AdjustAnnualLeaves(AddMultiAnnualLeaveTransactionViewModel model)
        {
            CommandResult result = new CommandResult
            {
                Code = -1,
                Message = Messages.InvalidInputData
            };

            if (model != null && ModelState.IsValid)
            {
                var command = new AdjustAnnualLeavesCommand
                {
                    EmployeeId = model.EmpIds,
                    Description = model.Description,
                    FromDate = model.AffectedFrom,
                    DeductDays = model.TotalDays > 0 ? 0 : -model.TotalDays,
                    CreditDays = model.TotalDays > 0 ? model.TotalDays : 0,
                    CreatedBy = CurrentUser.Id
                };

                result = await baseService.ExecuteNonQueryAsync(command);
            }

            return Json(result);
        }

        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Create(int employeeId)
        {
            var criteria = new GetEmployeeByIdCriteria { EmployeeId = employeeId };
            ViewBag.Employee = await employeeService.GetEmployeeById(criteria);

            return View(new AddAnnualLeaveTransactionViewModel
            {
                EmployeeId = employeeId,
                AffectedFrom = DateTimeOffset.Now.DateTime.ToClientTimeZone()
            });
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> Create(AddAnnualLeaveTransactionViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var criteria = new InsertAnnualLeaveTransactionCommand
                {
                    EmployeeId = model.EmployeeId,
                    Description = model.Description,
                    FromDate = model.AffectedFrom,
                    ToDate = model.AffectedFrom,
                    DeductDays = model.TotalDays > 0 ? 0 : -model.TotalDays,
                    CreditDays = model.TotalDays > 0 ? model.TotalDays : 0,
                    CreatedBy = CurrentUser.Id
                };

                CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);
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
                OnDate = DateTimeOffset.Now.DateTime.ToClientTimeZone(),
                TotalDays = decimal.Parse("0.5"),
                Description = Labels.LeaveConversionDescription
            });
        }

        public PartialViewResult CreateLeaveDeduction()
        {
            return PartialView(new LeaveDeductionViewModel
            {
                DeductDate = DateTimeOffset.Now.DateTime.ToClientTimeZone(),
                TotalDays = decimal.Parse("0.5"),
                Description = Labels.LeaveDeductionDescription,
                IsAnnual = true
            });
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> Delete(int leaveTransactionId)
        {
            var command = new DeleteAnnualLeaveTransactionCommand
            {
                LeaveTransactionId = leaveTransactionId,
                ModifiedBy = CurrentUser.Id
            };
            CommandResult result = await baseService.ExecuteNonQueryAsync(command);
            return Json(result);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> DeleteLeaveConversion(int leaveConversionId)
        {
            var command = new DeleteLeaveConversionCommand
            {
                LeaveConversionId = leaveConversionId,
                ModifiedBy = CurrentUser.Id
            };

            return Json(await baseService.ExecuteNonQueryAsync(command));
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> DeleteLeaveDeduction(int leaveDeductionId)
        {
            var command = new DeleteLeaveDeductionCommand
            {
                LeaveDeductionId = leaveDeductionId,
                ModifiedBy = CurrentUser.Id
            };

            return Json(await baseService.ExecuteNonQueryAsync(command));
        }

        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Edit(int leaveTransactionId)
        {
            var criteria = new GetAnnualLeaveByIdCriteria
            {
                LeaveTransactionId = leaveTransactionId
            };
            var leaveTransaction = await baseService.GetDataEntityAsync<AnnualLeaveTransaction>(criteria);

            var model = new EditAnnualLeaveTransactionViewModel
            {
                LeaveTransactionId = leaveTransaction.LeaveTransactionId,
                TotalDays = leaveTransaction.TotalDays,
                AffectedFrom = leaveTransaction.AffectedFrom,
                Description = leaveTransaction.Description,
                EmployeeId = leaveTransaction.EmpId,
            };
            return View(model);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> Edit(EditAnnualLeaveTransactionViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var command = new UpdateAnnualLeaveTransactionCommand
                {
                    LeaveTransactionId = model.LeaveTransactionId,
                    Description = model.Description,
                    FromDate = model.AffectedFrom,
                    DeductDays = model.TotalDays > 0 ? 0 : -model.TotalDays,
                    CreditDays = model.TotalDays > 0 ? model.TotalDays : 0,
                    ModifiedBy = CurrentUser.Id
                };
                CommandResult result = await baseService.ExecuteNonQueryAsync(command);
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
                OnDate = DateTimeOffset.Now.DateTime.ToClientTimeZone(),
                TotalDays = decimal.Parse("0.5"),
                Description = Labels.LeaveConversionDescription
            });
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> EditLeaveConversion(LeaveConversionViewModel model)
        {
            CommandResult result;
            if (model != null && ModelState.IsValid)
            {
                var criteria = new UpdateLeaveConversionCommand
                {
                    LeaveConversionId = model.LeaveConversionId,
                    Description = model.Description,
                    Date = model.OnDate,
                    TotalDays = model.TotalDays,
                    ModifiedBy = CurrentUser.Id
                };
                result = await baseService.ExecuteNonQueryAsync(criteria);
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
                DeductDate = DateTimeOffset.Now.DateTime.ToClientTimeZone(),
                TotalDays = decimal.Parse("0.5"),
                Description = Labels.LeaveDeductionDescription,
                IsAnnual = true
            });
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> EditLeaveDeduction(LeaveDeductionViewModel model)
        {
            CommandResult result;
            if (model != null && ModelState.IsValid)
            {
                var criteria = new UpdateLeaveDeductionCommand
                {
                    LeaveDeductionId = model.LeaveDeductionId,
                    Description = model.Description,
                    DeductDate = model.DeductDate,
                    IsAnnual = model.IsAnnual,
                    TotalDays = model.TotalDays,
                    ModifiedBy = CurrentUser.Id
                };
                result = await baseService.ExecuteNonQueryAsync(criteria);
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

        [HttpPost]
        public async Task<ActionResult> ViewHistoryAnnualByMonth(MonthlyAnnualLeaveViewModel model)
        {
            IEnumerable<AnnualLeaveTransaction> leaveTransactions = new List<AnnualLeaveTransaction>();
            if (model != null)
            {
                var criteria = new GetAnnualLeaveTransactionByMonthCriteria
                {
                    EmpId = model.EmployeeId,
                    Month = model.Month,
                    Year = model.Year
                };

                leaveTransactions = await baseService.GetDataAsync<AnnualLeaveTransaction>(criteria);
            }

            return PartialView("_EmployeeBalanceHis", leaveTransactions);
        }

        [HttpPost]
        public async Task<ActionResult> ViewDetailCreditDaysInYear(int employeeId, int year)
        {
            var criteria = new GetAnnualLeaveTransactionCriteria
            {
                EmpId = employeeId,
                Year = year,
                ExcludedRequestTransactions = true
            };

            var leaveTransactions = await baseService.GetDataAsync<AnnualLeaveTransaction>(criteria);

            return PartialView("_EmployeeBalanceHis", leaveTransactions);
        }

        public async Task<IActionResult> Employee(int employeeId, int year)
        {
            var model = new EmployeeAnnualLeaveViewModel
            {
                EmployeeId = employeeId,
                Year = year,
                IncludeRequestTrans = true
            };

            var getEmployeeCriteria = new GetEmployeeByIdCriteria { EmployeeId = employeeId };
            Employee employee = await employeeService.GetEmployeeById(getEmployeeCriteria);

            if (employee == null || employee.Id == 0)
            {
                return RedirectToAction("NoData", "Home");
            }
            else
            {
                model.NickName = employee.NickName;
            }

            return View("Employee", model);
        }

        [HttpPost]
        public async Task<IActionResult> Employee(EmployeeAnnualLeaveViewModel model)
        {
            IEnumerable<AnnualLeaveTransaction> leaveTransactions = new List<AnnualLeaveTransaction>();
            if (model != null)
            {
                var criteria = new GetAnnualLeaveTransactionCriteria
                {
                    EmpId = model.EmployeeId,
                    Year = model.Year,
                    ExcludedRequestTransactions = !model.IncludeRequestTrans
                };

                leaveTransactions = await baseService.GetDataAsync<AnnualLeaveTransaction>(criteria);
            }

            return PartialView("_Employee", leaveTransactions);
        }

        public async Task<IActionResult> Index()
        {
            var model = new LeaveSummaryViewModel
            {
                Year = DateTimeOffset.Now.DateTime.ToClientTimeZone().Year,
                EmployeeId = -1,
                Teams = await teamService.GetTeam(),
                Departments = await departmentService.GetAll(),
                ShowOpenAndCloseAnnualBalance = await GetOpenAndCloseAnnualBalance()
            };

            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(LeaveSummaryViewModel model)
        {
            var leaveSummary = new List<LeaveSummary>();
            if (model != null)
            {
                var criteria = new GetAnnualLeaveSummaryCriteria
                {
                    EmployeeId = model.EmployeeId != -1 ? new int?(model.EmployeeId) : null,
                    GroupId = model.TeamId,
                    DepartmentId = model.DepartmentId,
                    Year = model.Year,
                    MinAnnualBalance = model.MinAnnualBalance
                };
                leaveSummary = (await baseService.GetDataAsync<LeaveSummary>(criteria)).ToList();

                ViewBag.Year = model.Year;
            }

            return PartialView("_Index", leaveSummary);
        }

        [HttpPost]
        public async Task<IActionResult> ExportExcel(LeaveSummaryViewModel model)
        {
            var leaveSummary = new List<LeaveSummary>();
            if (model != null)
            {
                var criteria = new GetAnnualLeaveSummaryCriteria
                {
                    EmployeeId = model.EmployeeId != -1 ? new int?(model.EmployeeId) : null,
                    GroupId = model.TeamId,
                    DepartmentId = model.DepartmentId,
                    Year = model.Year,
                    MinAnnualBalance = model.MinAnnualBalance
                };
                leaveSummary = (await baseService.GetDataAsync<LeaveSummary>(criteria)).ToList();
            }

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "ExcelTemplates", "LeaveSummaryReport.xlsx");
            var excel = new LeaveSummaryExcel(templatePath, leaveSummary, model.Year, CurrentUser.HasHRRole);
            var fileBytes = excel.Export();

            var fileName = $"AnnualLeaveSummary_{model.Year}_{DateTimeOffset.Now.DateTime.ToClientTimeZone():yyyyMMdd}.xlsx";
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost]
        public async Task<JsonResult> LeaveDeductionDetail(LeaveConversionDetailViewModel model)
        {
            IEnumerable<LeaveConversionDetail> leaveDeductions = new List<LeaveConversionDetail>();
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

                leaveDeductions = await baseService.GetDataAsync<LeaveConversionDetail>(criteria);
            }

            return Json(new { data = leaveDeductions });
        }

        public async Task<IActionResult> LeaveDeductionDetail(
            DateTime fromDate,
            DateTime toDate,
            int? employeeId = null,
            int? teamId = null)
        {
            return View(await GetLeaveDetailModel(fromDate, toDate, employeeId, teamId));
        }

        [HrmAuthorize(UserRoles = "HR")]
        public PartialViewResult MultipleInsert()
        {
            return PartialView(new AddMultiAnnualLeaveTransactionViewModel
            {
                AffectedFrom = DateTimeOffset.Now.DateTime.ToClientTimeZone()
            });
        }

        [HrmAuthorize(UserRoles = "HR")]
        public PartialViewResult MultipleReset()
        {
            return PartialView(new AddMultiAnnualLeaveTransactionViewModel
            {
                AffectedFrom = DateTimeOffset.Now.DateTime.ToClientTimeZone()
            });
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> OpenAnnualLeaves()
        {
            var criteria = new InsertAnnualBalanceForNewYearCommand
            {
                Year = ShowOpenAndCloseAnnualBalance.ToDate.Year
            };

            return Json(await baseService.ExecuteNonQueryAsync(criteria));
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<JsonResult> ResetAnnualLeaves(int minbalance, string descriptionReset)
        {
            var criteria = new GetAnnualLeaveSummaryCriteria
            {
                EmployeeId = null,
                GroupId = null,
                Year = ShowOpenAndCloseAnnualBalance.FromDate.Year
            };

            var leaveSummaries = await baseService.GetDataAsync<LeaveSummary>(criteria);
            var leaveSummariesFilter = leaveSummaries.Where(m => m.LeaveBalance > minbalance).ToList();

            var resetAnnualLeaves = new ResetAnnualLeavesCommand
            {
                Description = descriptionReset,
                FromDate = new DateTimeOffset(ShowOpenAndCloseAnnualBalance.FromDate.Year, 12, 31, 0, 0, 0, TimeSpan.Zero).DateTime,
                ResetDays = minbalance,
                CreatedBy = CurrentUser.Id,
                LeaveSummaries = leaveSummariesFilter
            };

            CommandResult result = await baseService.ExecuteNonQueryAsync(resetAnnualLeaves);

            return Json(result);
        }

        private async Task<LeaveConversionDetailViewModel> GetLeaveDetailModel(DateTime fromDate, DateTime toDate, int? employeeId, int? teamId)
        {
            IList<Team> teams = await teamService.GetTeam();

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

        private async Task<ShowOpenAndCloseAnnualBalance> GetOpenAndCloseAnnualBalance()
        {
            var criteria = new GetOpenAndCloseAnnualBalanceCriteria
            {
                OpenDate = new DateTimeOffset(ShowOpenAndCloseAnnualBalance.ToDate.Year, 1, 1, 0, 0, 0, TimeSpan.Zero).DateTime,
                CloseDate = new DateTimeOffset(ShowOpenAndCloseAnnualBalance.FromDate.Year, 12, 31, 0, 0, 0, TimeSpan.Zero).DateTime,
            };

            var model = await baseService.GetDataEntityAsync<ShowOpenAndCloseAnnualBalance>(criteria);

            return model;
        }
    }
}