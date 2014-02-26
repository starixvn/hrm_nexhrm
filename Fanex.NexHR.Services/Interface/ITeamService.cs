// --------------------------------------------------------------------------
// <copyright file="ITeamService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Services.Interface
{
    using System;
    using System.Collections.Generic;
    using Fanex.NAP.Framework.Data;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.ViewModels;

    public interface ITeamService
    {
        Team Get(ICriteria criteria);

        IList<Team> GetAll(ICriteria criteria);

        IList<Team> GetTeamManagedByEmp(ICriteria criteria);

        IList<Team> GetSubTeamsOfEmployee(ICriteria criteria);

        IList<Team> GetTeamsHandleRequestsOfEmployee(ICriteria criteria);

        int AddNewTeam(ICriteria criteria);

        bool UpdateTeam(ICriteria criteria);

        bool DeleteTeam(ICriteria criteria, out int errCode, out string message);
    }
}