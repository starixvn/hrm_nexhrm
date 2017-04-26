namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Criteria.Team;
    using Models;
    using Resources;
    using Services;
    using Services.Interfaces;
    using ViewModels;

    public class TeamController : BaseController
    {
        private readonly ITeamService _teamService;
        private readonly IDepartmentService _departmentService;
        private readonly ICommandService _commandService;

        public TeamController()
            : this(null, null, null)
        {
        }

        public TeamController(
            ITeamService teamService,
            IDepartmentService departmentService,
            ICommandService commandService)
        {
            _teamService = teamService ?? new TeamService();
            _departmentService = departmentService ?? new DepartmentService();
            _commandService = commandService ?? new CommandService();
        }

        public ActionResult Index()
        {
            IList<Team> teams = _teamService.GetTeam();

            return View(teams);
        }

        [Authorize(Roles = "HR")]
        public ActionResult Create()
        {
            var model = new TeamViewModel
            {
                Departments = _departmentService.GetAll(),
                Teams = _teamService.GetTeam()
            };

            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult Create(TeamViewModel model)
        {
            if (model != null)
            {
                if (ModelState.IsValid)
                {
                    var criteria = new InsertTeamCriteria()
                    {
                        Code = model.TeamCode,
                        Name = model.TeamName,
                        LeaderId = model.LeaderId,
                        AssistantLeaderId = model.AssistantLeaderId ?? default(int),
                        DepartmentId = model.DepartmentId,
                        ParentId = model.ParentId,
                        CreatedBy = CurrentUser.Id
                    };

                    CommandResult result = _commandService.Insert(criteria);

                    if (result.IsSuccessful)
                    {
                        ViewBag.Inserted = true;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Message);
                    }
                }

                model.Departments = _departmentService.GetAll();
                model.Teams = _teamService.GetTeam();
            }

            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult Edit(int id)
        {
            var criteria = new GetTeamByIdCriteria
            {
                Id = id
            };
            Team team = _teamService.GetById(criteria);

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
                model.Departments = _departmentService.GetAll();
                model.Teams = _teamService.GetTeam().Where(p => p.Id != id).ToList();
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult Edit(TeamViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.AssistantLeader))
                {
                    model.AssistantLeaderId = null;
                }

                var criteria = new UpdateTeamCriteria()
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

                CommandResult result = _commandService.Update(criteria);

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

            model.Departments = _departmentService.GetAll();
            model.Teams = _teamService.GetTeam().Where(p => p.Id != model.Id).ToList();

            return View(model);
        }

        [Authorize(Roles = "HR")]
        [HttpPost]
        public JsonResult Delete(int id)
        {
            var criteria = new DeleteTeamCriteria()
            {
                Id = id,
                ModifiedBy = CurrentUser.Id,
            };

            CommandResult result = _commandService.Delete(criteria);
            return Json(result);
        }
    }
}