namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Criteria.Team;
    using Fanex.NexHR.Models.Attributes;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Resources;
    using Services.Interfaces;
    using ViewModels;

    public class TeamController : BaseController
    {
        private readonly ITeamService teamService;
        private readonly IDepartmentService departmentService;
        private readonly ICommandService commandService;

        public TeamController(ISessionService sessionService,
            ITeamService teamService,
            IDepartmentService departmentService,
            ICommandService commandService) : base(sessionService)
        {
            this.teamService = teamService;
            this.departmentService = departmentService;
            this.commandService = commandService;
        }

        public async Task<ActionResult> Index()
        {
            IList<Team> teams = await teamService.GetTeam();

            return View(teams);
        }

        [HrmAuthorize(Roles = "HR")]
        public async Task<ActionResult> Create()
        {
            var model = new TeamViewModel
            {
                Departments = await departmentService.GetAll(),
                Teams = await teamService.GetTeam()
            };

            return View(model);
        }

        [HttpPost]
        [HrmAuthorize(Roles = "HR")]
        public async Task<ActionResult> Create(TeamViewModel model)
        {
            if (model != null)
            {
                if (ModelState.IsValid)
                {
                    var criteria = new InsertTeamCriteria
                    {
                        Code = model.TeamCode,
                        Name = model.TeamName,
                        LeaderId = model.LeaderId,
                        AssistantLeaderId = model.AssistantLeaderId ?? default(int),
                        DepartmentId = model.DepartmentId,
                        ParentId = model.ParentId,
                        CreatedBy = CurrentUser.Id
                    };

                    CommandResult result = commandService.ExecuteNonQuery(criteria);

                    if (result.IsSuccessful)
                    {
                        ViewBag.Inserted = true;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Message);
                    }
                }

                model.Departments = await departmentService.GetAll();
                model.Teams = await teamService.GetTeam();
            }

            return View(model);
        }

        [HrmAuthorize(Roles = "HR")]
        public async Task<ActionResult> Edit(int id)
        {
            var criteria = new GetTeamByIdCriteria
            {
                Id = id
            };
            Team team = await teamService.GetById(criteria);

            TeamViewModel model = null;
            if (team != null)
            {
                model = new TeamViewModel
                {
                    TeamCode = team.Code,
                    TeamName = team.Name,
                    DepartmentId = team.Department?.Id ?? 0,
                    ParentId = team.Parent?.Id ?? 0,
                    Leader = team.Leader.DisplayName,
                    LeaderId = team.Leader.Id,
                    AssistantLeader = team.AssistantLeader != null ? team.AssistantLeader.DisplayName : string.Empty,
                    AssistantLeaderId = team.AssistantLeader?.Id ?? 0,
                };
            }

            if (model != null)
            {
                var teams = await teamService.GetTeam();
                model.Departments = await departmentService.GetAll();
                model.Teams = teams.Where(p => p.Id != id).ToList();
            }

            return View(model);
        }

        [HttpPost]
        [HrmAuthorize(Roles = "HR")]
        public async Task<ActionResult> Edit(TeamViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.AssistantLeader))
                {
                    model.AssistantLeaderId = null;
                }

                var criteria = new UpdateTeamCriteria
                {
                    Id = model.Id,
                    Code = model.TeamCode,
                    Name = model.TeamName,
                    DepartmentId = model.DepartmentId,
                    ParentId = model.ParentId,
                    LeaderId = model.LeaderId,
                    AssistantLeaderId = model.AssistantLeaderId ?? default(int),
                    ModifiedBy = CurrentUser.Id
                };

                CommandResult result = commandService.ExecuteNonQuery(criteria);

                if (result.IsSuccessful)
                {
                    ViewBag.Updated = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Messages.InvalidInputData);
            }

            var teams = await teamService.GetTeam();
            model.Departments = await departmentService.GetAll();
            model.Teams = teams.Where(p => p.Id != model.Id).ToList();

            return View(model);
        }

        [HrmAuthorize(Roles = "HR")]
        [HttpPost]
        public JsonResult Delete(int id)
        {
            var criteria = new DeleteTeamCriteria
            {
                Id = id,
                ModifiedBy = CurrentUser.Id,
            };

            CommandResult result = commandService.ExecuteNonQuery(criteria);
            return Json(result);
        }
    }
}