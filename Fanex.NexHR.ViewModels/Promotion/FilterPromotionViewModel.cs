namespace Fanex.NexHR.ViewModels.Promotion
{
    using Fanex.NexHR.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public class FilterPromotionViewModel
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int? EmpId { get; set; }

        public string Employee { get; set; }

        public int? TeamId { get; set; }

        [Display(Name = "Team")]
        public IList<Team> Teams { get; set; }
    }
}
