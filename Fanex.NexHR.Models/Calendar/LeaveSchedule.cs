using System;

namespace Fanex.NexHR.Models.Calendar
{
    public class LeaveSchedule
    {
        public Employee Employee { get; set; }

        public DateTime WorkDate { get; set; }

        public bool IsLeaveDay { get; set; }

        public bool IsHalfDayLeave { get; set; }

        public double LeaveDay { get; set; }

        public string LeaveType { get; set; }
    }
}