// --------------------------------------------------------------------------
// <copyright file="LeaveSummaryViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
    public class LeaveSummaryViewModel
    {
        public int Year { get; set; }

        public string Team { get; set; }

        public int? TeamId { get; set; }

        public string Employee { get; set; }

        public int EmployeeId { get; set; }

        public int MinAnnualBalance { get; set; }
    }
}
