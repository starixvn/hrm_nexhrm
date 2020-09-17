namespace Fanex.NexHR.ViewModels.YearlyNexRequest
{
    using Fanex.NexHR.Models;
    using System.Collections.Generic;

    public class YearlyNexRequestViewModel
   {
       public IList<Team> Teams { get; set; }

        public int? TeamId { get; set; }

        public string Employee { get; set; }

        public int EmployeeId { get; set; }

        public int Year { get; set; }
    }
}
