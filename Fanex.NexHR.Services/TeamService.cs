// --------------------------------------------------------------------------
// <copyright file="TeamService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
//   <change who="Bradley.Mac" date="2014.3.6">Apply Nap Framwork</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Services.Interfaces;

    /// <summary>
    /// Team service.
    /// </summary>
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;

        public TeamService(ITeamRepository teamRepository = null)
        {
            this._teamRepository = teamRepository ?? new TeamRepository();
        }

        /// <summary>
        /// Get Team By Id.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>Team entity.</returns>
        public Team Get(GetTeamByIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            Team team = new Team();
            if (criteria.IsValid())
            {
                team = _teamRepository.Get(criteria);
            }

            return team;
        }

        /// <summary>
        /// Get All Team.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List of Team Entity.</returns>
        public IList<Team> GetAll(GetAllTeamCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<Team> allTeam = new List<Team>();
            if (criteria.IsValid())
            {
                allTeam = this._teamRepository.GetAll(criteria);
            }

            return allTeam;
        }

        /// <summary>
        /// Get Team By Manager Id.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List of Team.</returns>
        public IList<Team> GetTeamManagedByEmp(GetTeamByManagerIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<Team> listTeam = new List<Team>();
            if (criteria.IsValid())
            {
                listTeam = this._teamRepository.GetTeamManagedByEmp(criteria);
            }

            return listTeam;
        }

        /// <summary>
        /// Get Sub Team Of Employee.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List of Team.</returns>
        public IList<Team> GetSubTeamsOfEmployee(GetSubTeamOfEmployeeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<Team> listSubTeam = new List<Team>();
            if (criteria.IsValid())
            {
                listSubTeam = this._teamRepository.GetSubTeamsOfEmployee(criteria);
            }

            return listSubTeam;
        }

        /// <summary>
        /// Add New Team.
        /// </summary>
        /// <param name="criteria">Criteria to Insert.</param>
        /// <returns>Value to check.</returns>
        public int AddNewTeam(AddNewTeamCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            int status = -1;
            if (criteria.IsValid())
            {
                status = this._teamRepository.AddNewTeam(criteria);
            }

            return status;
        }

        /// <summary>
        /// Update Team Information.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>Boolean to check.</returns>
        public bool UpdateTeam(UpdateTeamCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            bool updated = false;
            if (criteria.IsValid())
            {
                updated = this._teamRepository.UpdateTeam(criteria);
            }

            return updated;
        }

        /// <summary>
        /// Delete team.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <param name="errCode">The error code.</param>
        /// <param name="message">The error message.</param>
        /// <returns>The result.</returns>
        public bool DeleteTeam(DeleteTeamCriteria criteria, out int errCode, out string message)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            bool deleted = false;
            errCode = 0;
            message = string.Empty;
            if (criteria.IsValid())
            {
                deleted = this._teamRepository.DeleteTeam(criteria, out errCode, out message);
            }

            return deleted;
        }

        /// <summary>
        /// Get teams handle requests of employee.
        /// </summary>
        /// <param name="criteria">Criteria to query DB.</param>
        /// <returns>List of team.</returns>
        public IList<Team> GetTeamsHandleRequestsOfEmployee(GetTeamHandleRequestOfEmployee criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<Team> teamHandleRequest = new List<Team>();
            if (criteria.IsValid())
            {
                teamHandleRequest = this._teamRepository.GetTeamsHandleRequestsOfEmployee(criteria);
            }

            return teamHandleRequest;
        }
    }
}