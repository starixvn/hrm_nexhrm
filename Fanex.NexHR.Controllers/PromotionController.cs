namespace Fanex.NexHR.Controllers
{
    using System;
    using Fanex.NexHR.Criteria.Promotion;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Promotion;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using Fanex.NexHR.Controllers.Attributes;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;

    public class PromotionController : BaseController
    {
        private readonly ITeamService teamService;
        private readonly IEmployeeService employeeService;
        private readonly IBaseService baseService;
        private readonly IPromotionService promotionService;

        public PromotionController(ISessionService sessionService,
            ITeamService teamService,
            IEmployeeService employeeService,
            IPromotionService promotionService,
            IBaseService baseService) : base(sessionService)
        {
            this.employeeService = employeeService;
            this.baseService = baseService;
            this.teamService = teamService;
            this.promotionService = promotionService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new FilterPromotionViewModel
            {
                FromDate = DateTimeOffset.Now.Date,
                ToDate = DateTimeOffset.Now.Date,
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
                var criteria = new InsertPromotionCriteria
                {
                    EmpId = model.EmployeeId,
                    JobGradeId = model.JobGradeId,
                    OldJobGradeId = model.OldJobGradeId,
                    EffectiveDate = model.EffectiveDate,
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

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var promotion = await promotionService.GetById(new GetPromotionByIdCriteria { Id = id });
            var model = new EditPromotionViewModel();
            if (promotion != null)
            {
                model.Id = promotion.Id;
                model.OldJobGradeId = promotion.OldJobGradeId;
                model.JobGradeId = promotion.JobGradeId;
                model.EffectiveDate = promotion.AffectedDate;
                model.Employee = promotion.NickName + " (" + promotion.FullName + ")";
            }
            ViewBag.JobGrade = await promotionService.GetJobGrade();

            return PartialView(model);
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Edit(EditPromotionViewModel model)
        {
            var criteria = new UpdatePromotionCriteria
            {
                Id = model.Id,
                JobGradeId = model.JobGradeId,
                OldJobGradeId = model.OldJobGradeId,
                EffectiveDate = model.EffectiveDate,
                ModifiedBy = CurrentUser.Id
            };

            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);

            return Json(result);
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<CommandResult> Delete(int id)
        {
            var deletePromotionCriteria = new DeletePromotionCriteria { Id = id, ModifiedBy = CurrentUser.Id };
            CommandResult result = await baseService.ExecuteNonQueryAsync(deletePromotionCriteria);

            return result;
        }
    }
}
