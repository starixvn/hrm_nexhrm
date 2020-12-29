namespace Fanex.NexHR.Models.SyncData.Flow
{
    using System.Collections.Generic;

    public class EpaFlow
    {
        public IEnumerable<GroupFlow> GroupFlows { get; set; } = new List<GroupFlow>();
        public IEnumerable<EmployeeFlow> EmployeeFlows { get; set; } = new List<EmployeeFlow>();
    }
}