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

        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Create()
        {
            var model = new TeamViewModel
            {
                Departments = await departmentService.GetAll(),
                Teams = await teamService.GetTeam()
            };

            return View(model);
        }

        [HttpPost]
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Create(TeamViewModel model)
        {
            if (model != null)
            {
                if (ModelState.IsValid)
                {
                    var criteria = new InsertTeamCriteria
                    {
                        Code = model.TeamCode,
                        Name = model.TeamName,
                        DepartmentId = model.DepartmentId,
                        ParentId = model.ParentId,
                        CreatedBy = CurrentUser.Id
                    };

                    CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);

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

        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Edit(int id)
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
                    DepartmentId = team.DepartmentId,
                    ParentId = team.ParentId,
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
        [HrmAuthorize(UserRoles = "HR")]
        public async Task<IActionResult> Edit(TeamViewModel model)
        {
            if (ModelState.IsValid)
            {
                var criteria = new UpdateTeamCriteria
                {
                    Id = model.Id,
                    Code = model.TeamCode,
                    Name = model.TeamName,
                    DepartmentId = model.DepartmentId,
                    ParentId = model.ParentId,
                    ModifiedBy = CurrentUser.Id
                };

                CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);

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

        [HrmAuthorize(UserRoles = "HR")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var criteria = new DeleteTeamCriteria
            {
                Id = id,
                ModifiedBy = CurrentUser.Id,
            };

            CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);
            return Json(result);
        }
    }
}