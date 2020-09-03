namespace Fanex.NexHR.Models.Overtime
{
    using System;

    public class ReportOvertimeHRDetail
    {
        public int OvertimeId { get; set; }

        public string Note { get; set; }

        public DateTime Date { get; set; }

        public float Hour { get; set; }

        public float NexHour { get; set; }

        public Employee Employee { get; set; }
        
    }
}
