namespace Fanex.NexHR.ViewModels.EmployeeExperience
{
    using Fanex.NexHR.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public class EmployeeExperienceFilterViewModel
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int? EmployeeId { get; set; }

        public string Employee { get; set; }

        public int? TeamId { get; set; }

        [Display(Name = "Team")]
        public IList<Team> Teams { get; set; }

        public string Type { get; set; }

        public string Keyword { get; set; }
    }
}
