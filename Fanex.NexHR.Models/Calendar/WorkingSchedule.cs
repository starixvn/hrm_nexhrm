namespace Fanex.NexHR.Models.Calendar
{
    using System;

    public class WorkingSchedule
    {
        public Employee Employee { get; set; }

        public DateTime WorkDate { get; set; }

        public bool IsLeaveHalfDay { get; set; }

        public string Title { get; set; }
    }
}
