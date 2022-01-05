namespace Fanex.NexHR.Models
{
    using System;

    public class DashBoardCompanySpace
    {
        public Employee Employee { get; set; }

        public DateTime WorkDate { get; set; }

        public bool IsCurrent { get; set; }

        public bool IsWorkFromHome { get; set; }

        public short TypeId { get; set; }

        public float LeaveDay { get; set; }
    }
}
