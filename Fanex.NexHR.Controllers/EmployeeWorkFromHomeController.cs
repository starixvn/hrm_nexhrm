namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Controllers.Attributes;
    using Fanex.NexHR.Criteria.EmployeeWorkFromHome;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.EmployeeWorkFromHome;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading.Tasks;

    public class EmployeeWorkFromHomeController : BaseController
    {
        private readonly IEmployeeWorkFromHomeService employeeWorkFromHomeService;
        private readonly ITeamService teamService;
        private readonly IBaseService baseService;

        public EmployeeWorkFromHomeController(ISessionService sessionService,
            IEmployeeWorkFromHomeService employeeWorkFromHomeService,
            ITeamService teamService,
            IBaseService baseService) : base(sessionService)
        {
            this.employeeWorkFromHomeService = employeeWorkFromHomeService;
            this.teamService = teamService;
            this.baseService = baseService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new EmployeeWorkFromHomeFilterViewModel
            {
                FromDate = DateTimeOffset.Now.DateTime,
                ToDate = DateTimeOffset.Now.DateTime,
                Teams = await teamService.GetTeam(),
            };
            return View(model);
        }

        public async Task<IActionResult> GetEmployeeWorkFromHomeGrid(EmployeeWorkFromHomeFilterViewModel model)
        {
            int? statusNullable = null;
            var criteria = new GetEmployeeWorkFromHomeCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                GroupId = model.TeamId,
                EmpId = model.EmployeeId,
                StatusId = model.StatusId == 0 ? statusNullable : model.StatusId
            };
            var employees = await employeeWorkFromHomeService.GetAll(criteria);
            foreach (var employee in employees)
            {
                employee.Status = GetStatusName(employee.StatusId);
            }
            ViewBag.EmployeeId = CurrentUser.Id;
            return PartialView("_EmployeeWorkFromHomeGrid", employees);
        }

        [HrmAuthorize(UserRoles = "HR")]
        public IActionResult Create()
        {
            var model = new EmployeeWorkFromHomeViewModel
            {
                FromDate = DateTimeOffset.Now.DateTime,
                ToDate = DateTimeOffset.Now.DateTime
            };
            return View(model);
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Create(EmployeeWorkFromHomeViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var criteria = new InsertEmployeeWorkFromHomeCriteria
                {
                    EmpId = model.EmpId,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    CreatedBy = CurrentUser.Id,
                    Remark = model.Remark
                };

                var result = await baseService.ExecuteNonQueryAsync(criteria);
                if (result.IsSuccessful)
                {
                    ViewBag.Requested = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }
            return View(model);
        }

        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Edit(int id)
        {
            var employeeWorkFromHome = await employeeWorkFromHomeService.GetById(id);
            var model = new EmployeeWorkFromHomeViewModel
            {
                EmployeeWorkFromHomeId = employeeWorkFromHome.EmployeeWorkFromHomeId,
                EmpId = employeeWorkFromHome.EmpId,
                Employee = string.Format("{0} ({1})", employeeWorkFromHome.Nickname, employeeWorkFromHome.Fullname),
                FromDate = employeeWorkFromHome.FromDate,
                ToDate = employeeWorkFromHome.ToDate,
                Remark = employeeWorkFromHome.Remark
            };
            return View(model);
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Edit(EmployeeWorkFromHomeViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var criteria = new UpdateEmployeeWorkFromHomeCriteria
                {
                    EmployeeWorkFromHomeId = model.EmployeeWorkFromHomeId,
                    EmpId = model.EmpId,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    ModifiedBy = CurrentUser.Id,
                    Remark = model.Remark
                };

                var result = await baseService.ExecuteNonQueryAsync(criteria);
                if (result.IsSuccessful)
                {
                    ViewBag.Requested = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }
            return View(model);
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Delete(int id)
        {
            var criteria = new DeleteEmployeeWorkFromHomeCriteria { EmployeeWorkFromHomeId = id, DeletedBy = CurrentUser.Id };
            var result = await baseService.ExecuteNonQueryAsync(criteria);
            return Json(result);
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> ProcessRequestMultiple(string command, int[] requestIds)
        {
            var commandResult = new CommandResult();
            if (requestIds.Length > 0)
            {
                foreach (var id in requestIds)
                {
                    var criteria = new EmployeeWorkFromHomeApproveAndRejectRequestCriteria
                    {
                        WorkFromHomeRequestId = id,
                        StatusId = (byte)(command == Labels.Approve ? 2 : 3),
                        ProcessedBy = CurrentUser.Id,
                        ProcessedDate = DateTimeOffset.Now.DateTime
                    };
                    var result = await baseService.ExecuteNonQueryAsync(criteria);
                    if (!result.IsSuccessful)
                    {
                        commandResult.Code = result.Code;
                        commandResult.Message = result.Message;
                    }
                }
            }

            return Json(commandResult);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> ProcessRequest(string command, int id)
        {
            var criteria = new EmployeeWorkFromHomeApproveAndRejectRequestCriteria
            {
                WorkFromHomeRequestId = id,
                StatusId = (byte)(command == Labels.Approve ? 2 : 3),
                ProcessedBy = CurrentUser.Id,
                ProcessedDate = DateTimeOffset.Now.DateTime
            };
            var result = await baseService.ExecuteNonQueryAsync(criteria);
            return Json(result);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> ProcessDisapproveRequest(int id)
        {
            var criteria = new EmployeeWorkFromHomeDisApproveRequestCriteria
            {
                WorkFromHomeRequestId = id,
                ProcessedBy = CurrentUser.Id,
                ProcessedDate = DateTimeOffset.Now.DateTime
            };
            var result = await baseService.ExecuteNonQueryAsync(criteria);
            return Json(result);
        }

        private static string GetStatusName(byte Id)
        {
            const int secondCase = 2;
            const int threeCase = 3;

            switch (Id)
            {
                case 1:
                    return Labels.Pending;

                case secondCase:
                    return Labels.Approved;

                case threeCase:
                    return Labels.Rejected;

                default:
                    return string.Empty;
            }
        }
    }
}