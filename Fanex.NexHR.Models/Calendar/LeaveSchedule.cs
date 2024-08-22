namespace Fanex.NexHR.Models.Calendar
{
    using System;

    public class LeaveSchedule
    {
        public int EmpId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string TeamCode { get; set; }

        public DateTime WorkDate { get; set; }

        public bool IsLeaveDay { get; set; }

        public bool IsHalfDayLeave { get; set; }

        public double LeaveDay { get; set; }

        public string LeaveType { get; set; }
    }
}