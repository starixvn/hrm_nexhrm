// --------------------------------------------------------------------------
// <copyright file="ITeamService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
//   <change who="Bradley.Mac" date="2014.3.11">Apply Nap Framework</change>
// </history>

namespace Fanex.NexHR.Services.Interfaces
{
    using System.Collections.Generic;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;

    public interface ITeamService
    {
        Team GetById(GetTeamByIdCriteria criteria);

        IList<Team> GetAll(GetTeamCriteria criteria);

        IList<Team> GetTeamManagedByEmp(GetTeamByManagerIdCriteria criteria);

        IList<Team> GetSubTeamsOfEmployee(GetSubTeamOfEmployeeCriteria criteria);

        IList<Team> GetTeamsHandleRequestsOfEmployee(GetTeamHandleRequestOfEmployee criteria);

        int AddNewTeam(AddNewTeamCriteria criteria);

        bool UpdateTeam(UpdateTeamCriteria criteria);

        CommandResult DeleteTeam(DeleteTeamCriteria criteria);
    }
}