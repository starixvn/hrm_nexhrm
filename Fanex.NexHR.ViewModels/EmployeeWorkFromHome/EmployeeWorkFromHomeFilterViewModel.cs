namespace Fanex.NexHR.ViewModels.EmployeeWorkFromHome
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Models;

    public class EmployeeWorkFromHomeFilterViewModel
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int? EmployeeId { get; set; }

        public string Employee { get; set; }

        public int? TeamId { get; set; }

        [Display(Name = "Team")]
        public IList<Team> Teams { get; set; }

        public byte StatusId { get; set; }
    }
}
