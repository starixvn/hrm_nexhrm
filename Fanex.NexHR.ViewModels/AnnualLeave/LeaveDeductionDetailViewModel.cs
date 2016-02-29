namespace Fanex.NexHR.ViewModels.AnnualLeave
{
    using System;
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class LeaveDeductionDetailViewModel
    {
        public int EmployeeId { get; set; }

        public string Employee { get; set; }

        public int? TeamId { get; set; }

        public int DateOption { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public IList<Team> Teams { get; set; }
    }
}