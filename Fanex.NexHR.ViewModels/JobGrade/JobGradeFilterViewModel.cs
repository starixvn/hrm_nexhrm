namespace Fanex.NexHR.ViewModels.JobGrade
{
    using Fanex.NexHR.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class JobGradeFilterViewModel
    {

        public int? EmployeeId { get; set; }

        public string Employee { get; set; }

        public int? TeamId { get; set; }
        

        [Display(Name = "Team")]
        public IList<Team> Teams { get; set; }

    }
}
