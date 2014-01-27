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
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Team controller.
    /// </summary>
    [Authorize]
    public class TeamController : BaseController
    {
        private readonly ITeamService _teamService;
        private readonly ITeamRepository _teamRepository;

        public TeamController()
            : this(null)
        {
        }

        public TeamController(ITeamService teamService = null)
        {
            _teamService = teamService ?? new TeamService();
            _teamRepository = new TeamRepository();
        }

        /// <summary>
        /// Index page.
        /// </summary>
        /// <returns>Action result.</returns>
        public ActionResult Index()
        {
            IList<Team> teams = _teamRepository.GetAll();

            return View(teams);
        }

        /// <summary>
        /// Create team.
        /// </summary>
        /// <returns>Action result.</returns>
        [Authorize(Roles = "HR")]
        public ActionResult Create()
        {
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
                var id = _teamService.AddNewTeam(model);
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
            var team = _teamService.Get(id);

            TeamViewModel model = null;
            if (team != null)
            {
                model = new TeamViewModel
                {
                    TeamCode = team.Code,
                    TeamName = team.Name,
                    Leader = team.Leader.DisplayName,
                    LeaderId = team.Leader.Id,
                    AssistantLeader = team.AssistantLeader.DisplayName,
                    AssistantLeaderId = team.AssistantLeader.Id,
                    ModifiedDate = DateTime.Now,
                    ModifiedBy = this.CurrentUser.Id
                };
            }

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

                var result = _teamService.UpdateTeam(model);
                ViewBag.Updated = result;
            }
            else
            {
                ModelState.AddModelError(string.Empty, Resources.Messages.InvalidInputData);
            }

            return View(model);
        }
    }
}
