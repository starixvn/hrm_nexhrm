// --------------------------------------------------------------------------
// <copyright file="LateInEarlyOutSummaryViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.8.6">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels.Report
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class LateInEarlyOutSummaryViewModel
    {
        public int Year { get; set; }

        public int? TeamId { get; set; }

        public string Employee { get; set; }

        public int EmployeeId { get; set; }

        public IList<Team> TeamCollection { get; set; }
    }
}