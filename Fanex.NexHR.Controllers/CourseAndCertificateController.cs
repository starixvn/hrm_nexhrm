namespace Fanex.NexHR.Controllers
{
    using Fanex.NexHR.Core.Enums;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels.EmployeeWorkFromHome;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading.Tasks;

    public class CourseAndCertificateController : BaseController
    {
        private readonly ITeamService teamService;
        public CourseAndCertificateController(ISessionService sessionService,
            ITeamService teamService
            ) : base(sessionService)
        {
            this.teamService = teamService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new EmployeeWorkFromHomeFilterViewModel
            {
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                Teams = await teamService.GetTeam(),
                StatusId = (byte)RequestStatuse.All
            };
            return View(model);
        }

    }
}
