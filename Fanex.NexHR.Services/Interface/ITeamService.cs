// --------------------------------------------------------------------------
// <copyright file="ITeamService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
//   <change who="Bradley.Mac" date="2014.3.11">Apply Nap Framework</change>
// </history>

namespace Fanex.NexHR.Services.Interface
{
    using System.Collections.Generic;
    using Fanex.NAP.Framework.Data;
    using Fanex.NexHR.Criteria.Team;
    using Fanex.NexHR.Models;

    public interface ITeamService
    {
        Team Get(GetTeamByIdCriteria criteria);

        IList<Team> GetAll(GetAllTeamCriteria criteria);

        IList<Team> GetTeamManagedByEmp(GetTeamByManagerIdCriteria criteria);

        IList<Team> GetSubTeamsOfEmployee(GetSubTeamOfEmployeeCriteria criteria);

        IList<Team> GetTeamsHandleRequestsOfEmployee(GetTeamHandleRequestOfEmployee criteria);

        int AddNewTeam(AddNewTeamCriteria criteria);

        bool UpdateTeam(UpdateTeamCriteria criteria);

        bool DeleteTeam(DeleteTeamCriteria criteria, out int errCode, out string message);
    }
}