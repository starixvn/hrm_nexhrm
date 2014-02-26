// --------------------------------------------------------------------------
// <copyright file="ITeamRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Fanex.NAP.Framework.Data;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.ViewModels;

    public interface ITeamRepository
    {
        Team Get(ICriteria criteria);

        IList<Team> GetAll(ICriteria criteria);

        IList<Team> GetTeamManagedByEmp(ICriteria criteria);

        IList<Team> GetSubTeamsOfEmployee(ICriteria criteria);

        int AddNewTeam(ICriteria criteria);

        bool UpdateTeam(ICriteria criteria);

        bool DeleteTeam(ICriteria criteria, out int errorCode, out string message);

        IList<Team> GetTeamsHandleRequestsOfEmployee(ICriteria criteria);
    }
}
