namespace Fanex.NexHR.ViewModels.Overtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.NexHR.Models;

    public class OvertimeSummaryViewModel
    {
        public int? EmployeeId { get; set; }

        public string Employee { get; set; }

        public int? TeamId { get; set; }

        public int Year { get; set; }

        public IList<Team> Teams { get; set; }
    }
}
