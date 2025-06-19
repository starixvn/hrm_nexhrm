namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Criteria.Team;
    using Fanex.NexHR.Controllers.Attributes;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Resources;
    using Services.Interfaces;
    using ViewModels;

    public class TeamController : BaseController
    {
        private readonly ITeamService teamService;
        private readonly IDepartmentService departmentService;
        private readonly IBaseService baseService;

        public TeamController(ISessionService sessionService,
            ITeamService teamService,
            IDepartmentService departmentService,
            IBaseService baseService) : base(sessionService)
        {
            this.teamService = teamService;
            this.departmentService = departmentService;
            this.baseService = baseService;
        }

        public async Task<IActionResult> Index()
        {
            IList<Team> teams = await teamService.GetTeam();

            return View(teams);
        }
    }
}