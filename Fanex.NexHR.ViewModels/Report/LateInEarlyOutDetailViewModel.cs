namespace Fanex.NexHR.ViewModels.Report
{
    using System;
    using System.Collections.Generic;
    using Models;

    public class LateInEarlyOutDetailViewModel
    {
        public int EmployeeId { get; set; }

        public string Employee { get; set; }

        public int? TeamId { get; set; }

        public int DateOption { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public bool IncludeLeaveDeduction { get; set; }

        public IList<Team> Teams { get; set; }
    }
}