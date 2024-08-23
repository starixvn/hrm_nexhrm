namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Fanex.NexHR.Controllers.Attributes;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Criteria.WorkFromHomeRequest;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Models.WorkFromHomeRequest;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.WorkFromHomeRequest;
    using Microsoft.AspNetCore.Mvc;

    public class WorkFromHomeRequestController : BaseController
    {
        private readonly ITeamService teamService;
        private readonly IBaseService baseService;

        public WorkFromHomeRequestController(ISessionService sessionService,
            ITeamService teamService,
            IBaseService baseService) : base(sessionService)
        {
            this.teamService = teamService;
            this.baseService = baseService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetWorkFromHomeRequestGrid(int year)
        {
            var criteria = new GetWorkFromHomeRequestByEmpIdCriteria
            {
                FromDate = new DateTimeOffset(year, 1, 1, 0, 0, 0, TimeSpan.Zero).DateTime,
                ToDate = new DateTimeOffset(year, 12, 31, 0, 0, 0, TimeSpan.Zero).DateTime,
                EmpId = CurrentUser.Id,
            };

            var requests = await baseService.GetDataAsync<WorkFromHomeRequest>(criteria);
            return PartialView("_WorkFromHomeRequestGrid", requests);
        }

        public async Task<IActionResult> HRView()
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
            var criteria = new GetWorkFromHomeRequestCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                GroupId = model.TeamId,
                EmpId = model.EmployeeId,
                StatusId = model.StatusId == 0 ? statusNullable : model.StatusId
            };
            var workfromHomeRequests = await baseService.GetDataAsync<WorkFromHomeRequest>(criteria);
            ViewBag.EmployeeId = CurrentUser.Id;
            return PartialView("_EmployeeWorkFromHomeGrid", workfromHomeRequests);
        }

        [HrmAuthorize(UserRoles = "HR")]
        public IActionResult Create()
        {
            var model = new CreateWorkFromHomeRequestViewModel
            {
                FromDate = DateTimeOffset.Now.DateTime,
                ToDate = DateTimeOffset.Now.DateTime
            };
            return View(model);
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Create(CreateWorkFromHomeRequestViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var command = new InsertWorkFromHomeRequestCommand
                {
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    EmpId = CurrentUser.Id,
                    CreatedBy = CurrentUser.Id,
                    Remark = model.Remark,
                    StatusId = (int)RequestStatuse.Pending
                };

                var result = await baseService.ExecuteNonQueryAsync(command);
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

        public IActionResult HRCreate()
        {
            var model = new CreateWorkFromHomeRequestByHRViewModel
            {
                FromDate = DateTimeOffset.Now.DateTime,
                ToDate = DateTimeOffset.Now.DateTime
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> HRCreate(CreateWorkFromHomeRequestByHRViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var criteria = new InsertWorkFromHomeRequestCommand
                {
                    EmpId = model.EmpId,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    CreatedBy = CurrentUser.Id,
                    HRRemark = model.Remark,
                    StatusId = (int)RequestStatuse.Approve
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
            var request = await GetWorkFromHomeRequestById(id);
            var model = new EditWorkFromHomeRequestViewModel
            {
                Id = request.Id,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                Remark = request.Remark
            };
            return View(model);
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Edit(EditWorkFromHomeRequestViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var command = new UpdateWorkFromHomeRequestCommand
                {
                    WorkFromHomeRequestId = model.Id,
                    EmpId = CurrentUser.Id,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    ModifiedBy = CurrentUser.Id,
                    Remark = model.Remark
                };

                var result = await baseService.ExecuteNonQueryAsync(command);
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
        public async Task<IActionResult> HREdit(int id)
        {
            var request = await GetWorkFromHomeRequestById(id);
            var model = new EditWorkFromHomeRequestByHRViewModel
            {
                Id = request.Id,
                EmpId  = request.EmpId,
                Employee = request.Nickname,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                Remark = request.HRRemark
            };
            return View(model);
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> HREdit(EditWorkFromHomeRequestByHRViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var command = new UpdateWorkFromHomeRequestByHRCommand
                {
                    WorkFromHomeRequestId = model.Id,
                    EmpId = model.EmpId,
                    FromDate = model.FromDate,
                    ToDate = model.ToDate,
                    ModifiedBy = CurrentUser.Id,
                    HRRemark = model.Remark
                };

                var result = await baseService.ExecuteNonQueryAsync(command);
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
            var command = new DeleteWorkFromHomeRequestCommand { WorkFromHomeRequestId = id, DeletedBy = CurrentUser.Id };
            var result = await baseService.ExecuteNonQueryAsync(command);
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
                    var commandExec = new ProcessWorkFromHomeRequestCommand
                    {
                        WorkFromHomeRequestId = id,
                        StatusId = (byte)(command == Labels.Approve ? 2 : 3),
                        ProcessedBy = CurrentUser.Id
                    };
                    var result = await baseService.ExecuteNonQueryAsync(commandExec);
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
            var commandExec = new ProcessWorkFromHomeRequestCommand
            {
                WorkFromHomeRequestId = id,
                StatusId = (byte)(command == Labels.Approve ? 2 : 3),
                ProcessedBy = CurrentUser.Id
            };
            var result = await baseService.ExecuteNonQueryAsync(commandExec);
            return Json(result);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> ProcessDisapproveRequest(int id)
        {
            var command = new DisApproveWorkFromHomeRequestCommand
            {
                WorkFromHomeRequestId = id,
                ProcessedBy = CurrentUser.Id
            };
            var result = await baseService.ExecuteNonQueryAsync(command);
            return Json(result);
        }

        private async Task<WorkFromHomeRequest> GetWorkFromHomeRequestById(int id)
        {
            var criteria = new GetWorkFromHomeRequestByIdCriteria 
            { 
                 Id = id
            };
            var request = await baseService.GetDataEntityAsync<WorkFromHomeRequest>(criteria);
            return request;
        }
    }
}