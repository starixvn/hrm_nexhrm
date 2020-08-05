namespace Fanex.NexHR.Models.Overtime
{
    using System;

    public class ReportOvertimeHR
    {
        public int OvertimeId { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public float Hour { get; set; }

        public float NexHour { get; set; }

        public float AdjustedHour { get; set; }

        public string Title { get; set; }

        public bool IsLeaveHalfDay { get; set; }

        public Employee Employee { get; set; }

        public Employee CreatedBy { get; set; }
    }
}
