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
    using System.Web.Mvc;
    using Fanex.NexHR.Criteria.Department;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interfaces;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Team controller.
    /// </summary>
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

        /// <summary>
        /// Index page.
        /// </summary>
        /// <returns>Action result.</returns>
        public ActionResult Index()
        {
            var criteria = new GetAllTeamCriteria();
            IList<Team> teams = _teamService.GetAll(criteria);

            return View(teams);
        }

        /// <summary>
        /// Create team.
        /// </summary>
        /// <returns>Action result.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult Create()
        {
            var criteria = new GetAllDepartmentCriteria();
            IEnumerable<DepartmentModel> departments = _departmentService.GetAllDepartment(criteria);
            ViewBag.Departments = departments;

            var model = new TeamViewModel
            {
                CreatedDate = DateTime.Now,
                CreatedBy = this.CurrentUser.Id
            };
            return View(model);
        }

        /// <summary>
        /// Create team.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>Action result.</returns>
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
                    CreatedDate = model.CreatedDate,
                    CreatedBy = model.CreatedBy
                };

                var id = _teamService.AddNewTeam(criteria);
                if (id == -1)
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

            var getAllDepartmentCriteria = new GetAllDepartmentCriteria();
            IEnumerable<DepartmentModel> departments = _departmentService.GetAllDepartment(getAllDepartmentCriteria);
            ViewBag.Departments = departments;

            return View(model);
        }

        /// <summary>
        /// Edit team.
        /// </summary>
        /// <param name="id">The team ID.</param>
        /// <returns>Action result.</returns>
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
                    Leader = team.Leader.DisplayName,
                    LeaderId = team.Leader.Id,
                    AssistantLeader = team.AssistantLeader != null ? team.AssistantLeader.DisplayName : string.Empty,
                    AssistantLeaderId = team.AssistantLeader != null ? team.AssistantLeader.Id : 0,
                    ModifiedDate = DateTime.Now,
                    ModifiedBy = this.CurrentUser.Id
                };
            }

            var getAllDepartmentCriteria = new GetAllDepartmentCriteria();
            IEnumerable<DepartmentModel> departments = _departmentService.GetAllDepartment(getAllDepartmentCriteria);
            ViewBag.Departments = departments;

            return View(model);
        }

        /// <summary>
        /// Edit team.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>Action result.</returns>
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
                    LeaderId = model.LeaderId,
                    AssistantLeaderId = model.AssistantLeaderId != null ? model.AssistantLeaderId.Value : default(int),
                    ModifiedDate = model.ModifiedDate,
                    ModifiedBy = model.ModifiedBy
                };

                var result = _teamService.UpdateTeam(criteria);
                ViewBag.Updated = result;
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            var getAllDepartmentCriteria = new GetAllDepartmentCriteria();
            IEnumerable<DepartmentModel> departments = _departmentService.GetAllDepartment(getAllDepartmentCriteria);
            ViewBag.Departments = departments;

            return View(model);
        }
    }
}
