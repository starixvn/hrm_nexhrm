namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;

    public class TeamController : BaseController
    {
        private readonly ITeamService teamService;
        private readonly IDepartmentService departmentService;
        private readonly ICommandService commandService;

        public TeamController()
            : this(null, null, null)
        {
        }

        public TeamController(
            ITeamService teamService, 
            IDepartmentService departmentService,
            ICommandService commandService)
        {
            this.teamService = teamService ?? new TeamService();
            this.departmentService = departmentService ?? new DepartmentService();
            this.commandService = commandService ?? new CommandService();
        }

        public ActionResult Index()
        {
            IList<Team> teams = teamService.GetAll();

            return View(teams);
        }

        [Authorize(Roles = "HR")]
        public ActionResult Create()
        {
            var model = new TeamViewModel
            {
                CreatedBy = CurrentUser.Id,
                Departments = departmentService.GetAll(),
                Teams = teamService.GetAll()
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult Create(TeamViewModel model)
        {
            if (ModelState.IsValid)
            {
                var criteria = new InsertTeamCriteria()
                {
                    Code = model.TeamCode,
                    Name = model.TeamName,
                    LeaderId = model.LeaderId,
                    AssistantLeaderId = model.AssistantLeaderId != null ? model.AssistantLeaderId.Value : default(int),
                    DepartmentId = model.DepartmentId,
                    ParentId = model.ParentId,
                    CreatedBy = model.CreatedBy
                };

                CommandResult result = commandService.Insert(criteria);

                if (result.Code == 0)
                {
                    ViewBag.Inserted = true;
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

            model.Departments = departmentService.GetAll();
            model.Teams = teamService.GetAll();

            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult Edit(int id = -1)
        {
            var criteria = new GetTeamByIdCriteria
            {
                Id = id
            };
            var team = teamService.GetById(criteria);

            TeamViewModel model = null;
            if (team != null)
            {
                model = new TeamViewModel
                {
                    TeamCode = team.Code,
                    TeamName = team.Name,
                    DepartmentId = team.Department != null ? team.Department.DepartmentId : 0,
                    ParentId = team.Parent != null ? team.Parent.Id : 0,
                    Leader = team.Leader.DisplayName,
                    LeaderId = team.Leader.Id,
                    AssistantLeader = team.AssistantLeader != null ? team.AssistantLeader.DisplayName : string.Empty,
                    AssistantLeaderId = team.AssistantLeader != null ? team.AssistantLeader.Id : 0,
                    ModifiedBy = this.CurrentUser.Id
                };
            }

            model.Departments = departmentService.GetAll();
            model.Teams = teamService.GetAll().Where(p => p.Id != id).ToList();

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
                    AssistantLeaderId = model.AssistantLeaderId != null ? model.AssistantLeaderId.Value : default(int),
                    ModifiedBy = model.ModifiedBy,
                };

                CommandResult result = commandService.Update(criteria);

                if (result.Code == 0)
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

            model.Departments = departmentService.GetAll();
            model.Teams = teamService.GetAll().Where(p => p.Id != model.Id).ToList();

            return View(model);
        }
    }
}