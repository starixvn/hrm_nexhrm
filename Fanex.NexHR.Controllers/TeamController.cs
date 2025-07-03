namespace Fanex.NexHR.Controllers;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services.Interfaces;

public class TeamController(
    ISessionService sessionService,
    ITeamService teamService) : BaseController(sessionService)
{
    public async Task<IActionResult> Index()
    {
        IList<Team> teams = await teamService.GetTeam();

        return View(teams);
    }
}