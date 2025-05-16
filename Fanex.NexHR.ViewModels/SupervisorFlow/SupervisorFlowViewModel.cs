using System.Collections.Generic;
using Fanex.NexHR.Models;

namespace Fanex.NexHR.ViewModels.SupervisorFlow
{
    public class SupervisorFlowViewModel
    {
        public IEnumerable<Department> Departments { get; set; }

        public IList<Team> Teams { get; set; }
    }
}
