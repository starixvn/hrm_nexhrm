namespace Fanex.NexHR.ViewModels.Employee
{
    using Fanex.NexHR.Models;
    using System.Collections.Generic;

    public class StaffInfoViewModel
    {
        public string Keyword { get; set; }

        public int TeamId { get; set; }

        public IList<Team> Teams { get; set; }
    }
}