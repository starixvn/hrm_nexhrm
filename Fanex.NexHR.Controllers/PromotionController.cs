namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Controllers.Attributes;
    using Fanex.NexHR.Criteria.Promotion;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.Promotion;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

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
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
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
                EmpId = model.EmpId,
                GroupId = model.TeamId
            };
            var promotions = await promotionService.GetAll(criteria);

            return PartialView("_ListPromotion", promotions);
        }

    }
}
