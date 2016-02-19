namespace Fanex.NexHR.ViewModels.Report
{
    using System.Collections.Generic;
    using Fanex.NexHR.Models;

    public class LateInEarlyOutViewModel
    {
        public int Year { get; set; }

        public int? TeamId { get; set; }

        public string Employee { get; set; }

        public int EmployeeId { get; set; }

        public IList<Team> Teams { get; set; }
    }
}