namespace Fanex.NexHR.ViewModels.TrainingAndDevelopment
{
    using Fanex.NexHR.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public class TrainingAndDevelopmentFilterViewModel
    {
        public DateTime? FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int? EmployeeId { get; set; }

        public string Employee { get; set; }

        public int? TeamId { get; set; }
        public int? TypeId { get; set; }

        [Display(Name = "Team")]
        public IList<Team> Teams { get; set; }

        [Display(Name = "Type")]
        public IList<TrainingAndDevelopmentType> Types { get; set; }

        public string Name { get; set; }
    }
}
