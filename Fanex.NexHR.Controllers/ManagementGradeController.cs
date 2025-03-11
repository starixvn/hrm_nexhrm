namespace Fanex.NexHR.Controllers
{
    using System.Threading.Tasks;
    using Fanex.NexHR.Controllers.Attributes;
    using Fanex.NexHR.Criteria.ManagementGrade;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.ManagementGrade;
    using Microsoft.AspNetCore.Mvc;

    public class ManagementGradeController : BaseController
    {
        private readonly IPromotionService promotionService;
        private readonly IBaseService baseService;


        public ManagementGradeController(
            ISessionService sessionService,
            IPromotionService promotionService,
            IBaseService baseService) : base(sessionService)
        {
            this.promotionService = promotionService;
            this.baseService = baseService;
        }



        [HttpGet]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Update(int id, string nickname, string fullName, byte? managementGradeId)
        {

            var model = new UpdateManagementGradeViewModel
            {
                Employee = nickname + " (" + fullName + ")",
                EmployeeId = id,
                OldManagementGradeId = managementGradeId ?? 0
            };
            ViewBag.ManagementGrade = await promotionService.GetManagementGrade();

            return PartialView(model);
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Update(UpdateManagementGradeViewModel model)
        {
            var result = new CommandResult
            {
                Code = 1,
                Message = Messages.InvalidInputData
            };

            if (model != null && ModelState.IsValid)
            {
                var command = new InsertEmployeeManagementGradeCommand
                {
                    EmpId = model.EmployeeId,
                    ManagementGradeId = model.ManagementGradeId,
                    OldManagementGradeId = model.OldManagementGradeId,
                    IsPromotion = model.IsPromotion,
                    EffectiveDate = model.ManagementEffectiveDate,
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
    }
}
