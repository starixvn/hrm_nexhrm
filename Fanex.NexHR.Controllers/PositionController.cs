namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NexHR.Controllers.Attributes;
    using Fanex.NexHR.Criteria.Position;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.ViewModels;
    using Microsoft.AspNetCore.Mvc;
    using Services.Interfaces;

    public class PositionController : BaseController
    {
        private readonly IPositionService positionService;
        private readonly IBaseService baseService;

        public PositionController(ISessionService sessionService,
            IPositionService positionService,
            IBaseService baseService) : base(sessionService)
        {
            this.positionService = positionService;
            this.baseService = baseService;
        }

        [HrmAuthorize(UserRoles = "HR")]
        public IActionResult Index()
        {
            return View();
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> Index(string keyword)
        {

             var criteria = new GetAllPositionCriteria
             {
                Keyword = keyword
             };
            IEnumerable<Position> positions = await positionService.GetAll(criteria);

            return PartialView("_ListPosition", positions);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_Create", new CreatePositionViewModel());
        }

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> Create(string positionName, string positionCode, int? displayOrder)
        {
            var position = new InsertPositionCriteria
            {
                Name = positionName,
                Code = positionCode,
                DisplayOrder = displayOrder,
                ModifiedBy = CurrentUser.Id
            };
            CommandResult result = await baseService.ExecuteNonQueryAsync(position);

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var criteria = new GetPositionByIdCriteria
            {
                PositionId = id
            };

            EditPositionViewModel position = await positionService.GetById(criteria);

            return PartialView("_Edit", position);
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Edit(EditPositionViewModel model)
        {
            var criteria = new UpdatePositionCriteria
            {
                PositionId = model.PositionId,
                PositionName = model.PositionName,
                PositionCode = model.PositionCode,
                IsActive =  model.IsActive,
                DisplayOrder = model.DisplayOrder,
                ModifiedBy = CurrentUser.Id
            };

            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);

            return Json(result);
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Delete(int id)
        {
            var deletePositionCriteria = new DeletePositionCriteria { Id = id };
            CommandResult result = await baseService.ExecuteNonQueryAsync(deletePositionCriteria);

            return Json(result);
        }
    }
}