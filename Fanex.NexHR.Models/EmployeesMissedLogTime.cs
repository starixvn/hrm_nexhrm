using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NexHR.Models
{
    public class EmployeesMissedLogTime
    {
        public string TeamCode { get; set; }
        public int EmpID { get; set; }
        public string NickName { get; set; }
        public bool IsLeader { get; set; }
        public int MissedCount { get; set; }
        public string ConversationId { get; set; }
    }
}
