namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Controllers.Attributes;
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.EmployeeExperience;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading.Tasks;

    public class EmployeeExperienceController : BaseController
    {
        private readonly ITeamService teamService;
        public EmployeeExperienceController(ISessionService sessionService,
            ITeamService teamService
            ) : base(sessionService)
        {
            this.teamService = teamService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new EmployeeExperienceFilterViewModel
            {
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                Teams = await teamService.GetTeam(),
                TypeId = (byte)RequestStatuse.All
            };
            return View(model);
        }

        [HttpGet]
        [HrmAuthorize(UserRoles = "HR")]
        public IActionResult Update()
        {
            EmployeeExperienceInsertViewModel employeeExperience = null;
            return PartialView(employeeExperience);
        }
    }
}
