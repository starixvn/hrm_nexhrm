namespace Fanex.NexHR.Controllers
{
    using System.Threading.Tasks;
    using Fanex.NexHR.Controllers.Attributes;
    using Fanex.NexHR.Criteria.JobGrade;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.JobGrade;
    using Microsoft.AspNetCore.Mvc;

    public class JobGradeController : BaseController
    {
        private readonly ITeamService teamService;
        private readonly IPromotionService promotionService;
        private readonly IBaseService baseService;


        public JobGradeController(
            ISessionService sessionService,
            IPromotionService promotionService,
            IBaseService baseService,
            ITeamService teamService) : base(sessionService)
        {
            this.teamService = teamService;
            this.promotionService = promotionService;
            this.baseService = baseService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new JobGradeFilterViewModel { Teams = await teamService.GetTeam() };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetJobGrades(JobGradeFilterViewModel model)
        {
            var criteria = new GetEmployeeJobGradeCriteria 
            { 
                EmployeeId = model.EmployeeId,
                GroupId = model.TeamId
            };

            var employeeJobGrades = await baseService.GetDataEntityAsync<EmployeeJobGrade> (criteria);

            return PartialView("Views/JobGrade/_JobGradeGrid.cshtml", employeeJobGrades);
        }

        [HttpGet]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Update(int id, string nickname, string fullName, byte? JobGradeId)
        {

            var model = new UpdateJobGradeViewModel
            {
                Employee = nickname + " (" + fullName + ")",
                EmployeeId = id,
                OldJobGradeId =  JobGradeId ?? 0,
            };
            ViewBag.JobGrade = await promotionService.GetJobGrade();

            return PartialView(model);
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Update(UpdateJobGradeViewModel model)
        {
            var result = new CommandResult
            {
                Code = 1,
                Message = Messages.InvalidInputData
            };

            if (model != null && ModelState.IsValid)
            {
                var command = new InsertEmployeeJobGradeCommnand
                {
                    EmpId = model.EmployeeId,
                    JobGradeId = model.JobGradeId,
                    OldJobGradeId = model.OldJobGradeId,
                    IsPromotion = model.IsPromotion,
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
    }
}
