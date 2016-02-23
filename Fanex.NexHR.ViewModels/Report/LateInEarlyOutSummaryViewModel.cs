namespace Fanex.NexHR.ViewModels.Report
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class LateInEarlyOutSummaryViewModel
    {
        public int FromYear { get; set; }

        public int FromMonth { get; set; }

        public int ToYear { get; set; }

        public int ToMonth { get; set; }

        public int? TeamId { get; set; }

        public string Employee { get; set; }

        public int EmployeeId { get; set; }

        public bool IsUserWarning { get; set; }

        public IList<Team> Teams { get; set; }
    }
}