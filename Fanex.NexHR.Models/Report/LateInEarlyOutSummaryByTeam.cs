// --------------------------------------------------------------------------
// <copyright file="LateInEarlyOutSummaryByTeam.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.8.6">Created</change>
// </history>

namespace Fanex.NexHR.Models.Report
{
    using System.Collections.Generic;

    /// <summary>
    /// LateIn EarlyOut Summary By Team.
    /// </summary>
    public class LateInEarlyOutSummaryByTeam
    {
        public Team Team { get; set; }

        public List<LateInEarlyOutSummary> LateInEarlyOutSummaryInTeam { get; set; }
    }
}
