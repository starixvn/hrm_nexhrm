namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Fanex.NexHR.Controllers.Attributes;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.Promotion;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Promotion;
    using Microsoft.AspNetCore.Mvc;

    public class PromotionController : BaseController
    {
        private readonly ITeamService teamService;
        private readonly IBaseService baseService;
        private readonly IPromotionService promotionService;

        public PromotionController(ISessionService sessionService,
            ITeamService teamService,
            IPromotionService promotionService,
            IBaseService baseService) : base(sessionService)
        {
            this.baseService = baseService;
            this.teamService = teamService;
            this.promotionService = promotionService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new FilterPromotionViewModel
            {
                FromDate = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date,
                ToDate = DateTimeOffset.Now.DateTime.ToClientTimeZone().Date,
                Teams = await teamService.GetTeam()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetPromotions(FilterPromotionViewModel model)
        {
            var criteria = new GetPromotionCriteria
            {
                FromDate = model.FromDate,
                ToDate = model.ToDate,
                EmpId = model.EmployeeId,
                GroupId = model.TeamId
            };
            var promotions = await promotionService.GetAll(criteria);

            return PartialView("_ListPromotion", promotions);
        }

        [HttpGet]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Create()
        {
            ViewBag.JobGrade = await promotionService.GetJobGrade();
            return PartialView();
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Create(CreatePromotionViewModel model)
        {
            var result = new CommandResult
            {
                Code = 1,
                Message = Messages.InvalidInputData
            };

            if (model != null && ModelState.IsValid)
            {
                var command = new InsertPromotionCommand
                {
                    EmpId = model.EmployeeId,
                    JobGradeId = model.JobGradeId,
                    OldJobGradeId = model.OldJobGradeId,
                    EffectiveDate = model.EffectiveDate,
                    Note = model.Note,
                    ModifiedBy = CurrentUser.Id
                };
                result = await baseService.ExecuteNonQueryAsync(command);
                if (!result.IsSuccessful)
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            return Json(result);
        }

        [HttpGet]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Edit(int id)
        {
            var promotion = await promotionService.GetById(new GetPromotionByIdCriteria { Id = id });
            var model = new EditPromotionViewModel();
            if (promotion != null)
            {
                model.Id = promotion.Id;
                model.EmployeeId = promotion.EmployeeId;
                model.OldJobGradeId = promotion.OldJobGradeId;
                model.JobGradeId = promotion.JobGradeId;
                model.EffectiveDate = promotion.AffectedDate;
                model.Note = promotion.Note;
                model.Employee = promotion.NickName + " (" + promotion.FullName + ")";
            }
            ViewBag.JobGrade = await promotionService.GetJobGrade();

            return PartialView(model);
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Edit(EditPromotionViewModel model)
        {
            var command = new UpdatePromotionCommand
            {
                Id = model.Id,
                EmpId = model.EmployeeId,
                JobGradeId = model.JobGradeId,
                OldJobGradeId = model.OldJobGradeId,
                EffectiveDate = model.EffectiveDate,
                Note = model.Note,
                ModifiedBy = CurrentUser.Id
            };

            CommandResult result = await baseService.ExecuteNonQueryAsync(command);

            return Json(result);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<CommandResult> Delete(int id)
        {
            var command = new DeletePromotionCommand { Id = id, ModifiedBy = CurrentUser.Id };
            CommandResult result = await baseService.ExecuteNonQueryAsync(command);

            return result;
        }
    }
}
