namespace Fanex.NexHR.Controllers
{
    using System.Threading.Tasks;
    using Fanex.NexHR.Controllers.Attributes;
    using Fanex.NexHR.Criteria.SupervisorFlow;
    using Fanex.NexHR.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    public class SupervisorFlowController : BaseController
    {
        private readonly ISupervisorFlowService supervisorFlowService;
        private readonly ITeamService teamService;
        private readonly IBaseService baseService;


        public SupervisorFlowController(
            ISessionService sessionService,
            ISupervisorFlowService supervisorFlowService,
            IBaseService baseService,
            ITeamService teamService) : base(sessionService)
        {
            this.supervisorFlowService = supervisorFlowService;
            this.teamService = teamService;
            this.baseService = baseService;
        }

        [HttpGet]
        [HrmAuthorize(UserRoles = "HR")]
         public IActionResult Index(int chosenTab = 0)
        {
            switch (chosenTab)
            {
                case 1:
                    ViewBag.Tab = "Personal";
                    break;
                default:
                    ViewBag.Tab = "Team";
                    break;
            }

            return View();
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> TeamSupervisor(string keyword)
        {
            var criteria = new GetTeamSupervisorCriteria 
            { 
                Keyword = keyword
            };

            var teamSupervisors = await supervisorFlowService.GetTeamSupervisors(criteria);

            return PartialView("Views/SupervisorFlow/_Team.cshtml", teamSupervisors);
        }
    }
}
