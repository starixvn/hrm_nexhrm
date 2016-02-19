// --------------------------------------------------------------------------
// <copyright file="TeamController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria.Department;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;

    [Authorize]
    public class TeamController : BaseController
    {
        private readonly ITeamService _teamService;
        private readonly IDepartmentService _departmentService;

        public TeamController()
            : this(null, null)
        {
        }

        public TeamController(ITeamService teamService = null, IDepartmentService departmentService = null)
        {
            _teamService = teamService ?? new TeamService();
            _departmentService = departmentService ?? new DepartmentService();
        }

        public ActionResult Index()
        {
            var criteria = new GetTeamCriteria();
            IList<Team> teams = _teamService.GetAll(criteria);

            return View(teams);
        }

        [Authorize(Roles = "HR")]
        public ActionResult Create()
        {
            var model = new TeamViewModel
            {
                CreatedDate = DateTime.Now,
                CreatedBy = this.CurrentUser.Id
            };

            model.Departments = _departmentService.GetAllDepartment(new GetAllDepartmentCriteria()).ToList();
            model.Teams = _teamService.GetAll(new GetTeamCriteria());

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public ActionResult Create(TeamViewModel model)
        {
            if (ModelState.IsValid)
            {
                var criteria = new AddNewTeamCriteria()
                {
                    Code = model.TeamCode,
                    Name = model.TeamName,
                    LeaderId = model.LeaderId,
                    AssistantLeaderId = model.AssistantLeaderId != null ? model.AssistantLeaderId.Value : default(int),
                    DepartmentId = model.DepartmentId,
                    ParentId = model.ParentId,
                    CreatedDate = model.CreatedDate,
                    CreatedBy = model.CreatedBy
                };

                var id = _teamService.AddNewTeam(criteria);
                if (id.Equals(-1))
                {
                    ModelState.AddModelError(string.Empty, Resources.Messages.DuplicateTeamCode);
                }
                else
                {
                    ViewBag.Inserted = true;
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            model.Departments = _departmentService.GetAllDepartment(new GetAllDepartmentCriteria()).ToList();
            model.Teams = _teamService.GetAll(new GetTeamCriteria());

            return View(model);
        }

        [Authorize(Roles = "HR")]
        public ActionResult Edit(int id = -1)
        {
            var criteria = new GetTeamByIdCriteria
            {
                Id = id
            };
            var team = _teamService.GetById(criteria);

            TeamViewModel model = null;
            if (team != null)
            {
                model = new TeamViewModel
                {
                    TeamCode = team.Code,
                    TeamName = team.Name,
                    DepartmentId = team.Department != null ? team.Department.Id : 0,
                    ParentId = team.Parent != null ? team.Parent.Id : 0,
                    Leader = team.Leader.DisplayName,
                    LeaderId = team.Leader.Id,
                    AssistantLeader = team.AssistantLeader != null ? team.AssistantLeader.DisplayName : string.Empty,
                    AssistantLeaderId = team.AssistantLeader != null ? team.AssistantLeader.Id : 0,
                    ModifiedDate = DateTime.Now,
                    ModifiedBy = this.CurrentUser.Id
                };
            }

            model.Departments = _departmentService.GetAllDepartment(new GetAllDepartmentCriteria()).ToList();
            model.Teams = _teamService.GetAll(new GetTeamCriteria()).Where(p => p.Id != id).ToList();

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
                    ModifiedDate = model.ModifiedDate,
                    ModifiedBy = model.ModifiedBy,
                };

                var result = _teamService.UpdateTeam(criteria);
                ViewBag.Updated = result;
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            model.Departments = _departmentService.GetAllDepartment(new GetAllDepartmentCriteria()).ToList();
            model.Teams = _teamService.GetAll(new GetTeamCriteria()).Where(p => p.Id != model.Id).ToList();

            return View(model);
        }
    }
}