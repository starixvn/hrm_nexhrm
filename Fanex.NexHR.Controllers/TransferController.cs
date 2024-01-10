namespace Fanex.NexHR.Controllers
{
    using System;
    using Fanex.NexHR.Criteria.Transfer;
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using Fanex.NexHR.ViewModels.Transfer;
    using Fanex.NexHR.Controllers.Attributes;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;

    public class TransferController : BaseController
    {
        private readonly ITeamService teamService;
        private readonly IBaseService baseService;
        private readonly ITransferService transferService;

        public TransferController(ISessionService sessionService,
            ITeamService teamService,
            ITransferService transferService,
            IBaseService baseService) : base(sessionService)
        {
            this.baseService = baseService;
            this.teamService = teamService;
            this.transferService = transferService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new FilterTransferViewModel
            {
                FromDate = DateTimeOffset.Now.Date,
                ToDate = DateTimeOffset.Now.Date,
                Teams = await teamService.GetTeam()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetTransfers(FilterTransferViewModel model)
        {
            var criteria = new GetTransferCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                EmpId = model.EmployeeId,
                GroupId = model.TeamId
            };
            var transfer = await transferService.GetAll(criteria);

            return PartialView("_ListTransfer", transfer);
        }

        [HttpGet]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Teams = await teamService.GetTeam();
            return PartialView();
        }

         [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Create(CreateTransferViewModel model)
        {
            var result = new CommandResult
            {
                Code = 1,
                Message = Messages.InvalidInputData
            };

            if (model != null && ModelState.IsValid)
            {
                var criteria = new InsertTransferCriteria
                {
                    EmpId = model.EmployeeId,
                    GroupId = model.TeamId,
                    TransferDate = model.TransferDate,
                    ModifiedBy = CurrentUser.Id
                };
                result = await baseService.ExecuteNonQueryAsync(criteria);
                if (!result.IsSuccessful)
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            return Json(result);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<CommandResult> Delete(int id)
        {
            var deleteTransferCriteria = new DeleteTransferCriteria { Id = id };
            CommandResult result = await baseService.ExecuteNonQueryAsync(deleteTransferCriteria);

            return result;
        }
    }
}
