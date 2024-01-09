namespace Fanex.NexHR.Controllers
{
    using System.Threading.Tasks;
    using Fanex.NexHR.Controllers.Attributes;
    using Fanex.NexHR.Criteria.JobGrade;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.JobGrade;
    using Microsoft.AspNetCore.Mvc;

    public class JobGradeController : BaseController
    {
        private readonly IJobGradeService jobGradeService;
        private readonly ITeamService teamService;
        private readonly IPromotionService promotionService;
        private readonly IBaseService baseService;


        public JobGradeController(
            ISessionService sessionService,
            IJobGradeService jobGradeService,
            IPromotionService promotionService,
            IBaseService baseService,
            ITeamService teamService) : base(sessionService)
        {
            this.jobGradeService = jobGradeService;
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

            var employeeJobGrades = await jobGradeService.GetEmployeeJobGrade(criteria);

            return PartialView("Views/JobGrade/_JobGradeGrid.cshtml", employeeJobGrades);
        }

        [HttpGet]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Update(int id, string nickname, string fullName)
        {

            var model = new UpdateJobGradeViewModel
            {
                Employee = nickname + " (" + fullName + ")",
                EmployeeId = id
            };
            ViewBag.JobGrade = await promotionService.GetJobGrade();

            return PartialView(model);
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Update(UpdateJobGradeViewModel model)
        {
            var criteria = new UpdateJobGradeCriteria
            {
                EmpId = model.EmployeeId,
                JobGradeId = model.JobGradeId,
                EffectiveDate = model.EffectiveDate,
                Note = model.Note,
                ModifiedBy = CurrentUser.Id
            };

            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);

            return Json(result);
        }
    }
}
