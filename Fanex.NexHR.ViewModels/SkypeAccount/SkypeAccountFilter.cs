namespace Fanex.NexHR.ViewModels.SkypeAccount
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Models;

    public class SkypeAccountFilter
    {
        public int? EmployeeId { get; set; }

        public string Employee { get; set; }

        public int? TeamId { get; set; }

        [Display(Name = "Confirmed")]
        public bool? IsConfirmed { get; set; }

        public IList<Team> Teams { get; set; }
    }
}