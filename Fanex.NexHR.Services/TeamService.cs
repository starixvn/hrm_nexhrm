// --------------------------------------------------------------------------
// <copyright file="TeamService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.NAP.Framework.Data;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interface;
    using Fanex.NexHR.ViewModels;

    /// <summary>
    /// Team service.
    /// </summary>
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;

        public TeamService(ITeamRepository teamRespository = null)
        {
            this._teamRepository = teamRespository ?? new TeamRepository();
        }

        /// <summary>
        /// Get Team By Id.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>Team entity.</returns>
        public Team Get(ICriteria criteria)
        {
            Team team = _teamRepository.Get(criteria);

            return team;
        }

        /// <summary>
        /// Get group of employee.
        /// </summary>
        /// <param name="employeeId">The employee ID.</param>
        /// <returns>The return team.</returns>
        public Team GetGroupOfEmployee(int employeeId)
        {
            var team = new Team();

            if (employeeId > 0)
            {
                team = this._teamRepository.GetGroupOfEmployee(employeeId);
            }

            return team;
        }

        /// <summary>
        /// Get all team.
        /// </summary>
        /// <returns>List of team.</returns>
        public IList<Team> GetAll()
        {
            return this._teamRepository.GetAll();
        }

        /// <summary>
        /// Get team managed by employee.
        /// </summary>
        /// <param name="managerId">The manager ID.</param>
        /// <returns>List of team.</returns>
        public IList<Team> GetTeamManagedByEmp(int managerId)
        {
            return this._teamRepository.GetTeamManagedByEmp(managerId);
        }

        /// <summary>
        /// GEt sub teams of employee.
        /// </summary>
        /// <param name="employeeId">The employee ID.</param>
        /// <returns>List of team.</returns>
        public IList<Team> GetSubTeamsOfEmployee(int employeeId)
        {
            return this._teamRepository.GetSubTeamsOfEmployee(employeeId);
        }

        /// <summary>
        /// Add new team.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>The result.</returns>
        public int AddNewTeam(TeamViewModel model)
        {
            return this._teamRepository.AddNewTeam(model);
        }

        /// <summary>
        /// Update team.
        /// </summary>
        /// <param name="model">The model object.</param>
        /// <returns>The result.</returns>
        public bool UpdateTeam(TeamViewModel model)
        {
            return this._teamRepository.UpdateTeam(model);
        }

        /// <summary>
        /// Delete team.
        /// </summary>
        /// <param name="id">The team ID.</param>
        /// <param name="modifiedDate">The modified date.</param>
        /// <param name="modifiedby">The modified by.</param>
        /// <param name="errCode">The error code.</param>
        /// <param name="message">The error message.</param>
        /// <returns>The result.</returns>
        public bool DeleteTeam(int id, DateTime modifiedDate, int modifiedby, out int errCode, out string message)
        {
            return this._teamRepository.DeleteTeam(id, modifiedDate, modifiedby, out errCode, out message);
        }

        /// <summary>
        /// Get teams handle requests of employee.
        /// </summary>
        /// <param name="employeeId">The employee ID.</param>
        /// <returns>List of team.</returns>
        public IList<Team> GetTeamsHandleRequestsOfEmployee(int employeeId)
        {
            return this._teamRepository.GetTeamsHandleRequestsOfEmployee(employeeId);
        }
    }
}