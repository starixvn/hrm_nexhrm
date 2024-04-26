using Microsoft.AspNetCore.Mvc.Rendering;

namespace Fanex.NexHR.ViewModels.SupervisorFlow
{
    public class CreatePersonalViewModel
    {
        public int EmpId { get; set; }

        public string Employee { get; set; }

        public int[] ApproverIds { get; set; }

        public SelectList Employees { get; set; }

    }
}
