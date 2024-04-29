using Microsoft.AspNetCore.Mvc.Rendering;

namespace Fanex.NexHR.ViewModels.SupervisorFlow
{
    public class TeamViewModel
    {
        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public int[] ApproverIds { get; set; }

        public SelectList Employees { get; set; }
    }
}
