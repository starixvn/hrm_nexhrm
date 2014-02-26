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
        /// Get All Team.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List of Team Entity.</returns>
        public IList<Team> GetAll(ICriteria criteria)
        {
            return this._teamRepository.GetAll(criteria);
        }

        /// <summary>
        /// Get Team By Manager Id.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List of Team.</returns>
        public IList<Team> GetTeamManagedByEmp(ICriteria criteria)
        {
            return this._teamRepository.GetTeamManagedByEmp(criteria);
        }

        /// <summary>
        /// Get Sub Team Of Employee.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List of Team.</returns>
        public IList<Team> GetSubTeamsOfEmployee(ICriteria criteria)
        {
            return this._teamRepository.GetSubTeamsOfEmployee(criteria);
        }

        /// <summary>
        /// Add New Team.
        /// </summary>
        /// <param name="criteria">Criteria to Insert.</param>
        /// <returns>Value to check.</returns>
        public int AddNewTeam(ICriteria criteria)
        {
            return this._teamRepository.AddNewTeam(criteria);
        }

        /// <summary>
        /// Update Team Information.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>Boolean to check.</returns>
        public bool UpdateTeam(ICriteria criteria)
        {
            return this._teamRepository.UpdateTeam(criteria);
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
        public bool DeleteTeam(ICriteria criteria, out int errCode, out string message)
        {
            return this._teamRepository.DeleteTeam(criteria, out errCode, out message);
        }

        /// <summary>
        /// Get teams handle requests of employee.
        /// </summary>
        /// <param name="employeeId">The employee ID.</param>
        /// <returns>List of team.</returns>
        public IList<Team> GetTeamsHandleRequestsOfEmployee(ICriteria criteria)
        {
            return this._teamRepository.GetTeamsHandleRequestsOfEmployee(criteria);
        }
    }
}