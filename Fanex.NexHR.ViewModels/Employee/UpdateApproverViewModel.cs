namespace Fanex.NexHR.ViewModels.Employee
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Models;

    public class UpdateApproverViewModel
    {
        public int TeamId { get; set; }
        
        public int EmpId { get; set; }

        public int[] ApproverIds { get; set; }

        public string Employee { get; set; }

        public SelectList Employees { get; set; }

        public IEnumerable<Team> Teams { get; set; }
    }
}