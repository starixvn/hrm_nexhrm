namespace Fanex.NexHR.Controllers
{
    using System.Threading.Tasks;
    using Fanex.NexHR.Criteria.JobGrade;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.JobGrade;
    using Microsoft.AspNetCore.Mvc;

    public class JobGradeController : BaseController
    {
        private readonly IJobGradeService jobGradeService;
        private readonly ITeamService teamService;

        public JobGradeController(
            ISessionService sessionService,
            IJobGradeService jobGradeService,
            ITeamService teamService) : base(sessionService)
        {
            this.jobGradeService = jobGradeService;
            this.teamService = teamService;
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
    }
}
