namespace Fanex.NexHR.Models.Overtime
{
    using System;

    public class OvertimeRecord
    {
        public OvertimeRecord()
        {
            Date = DateTime.Now;
            Hour = 1;
        }

        public int OvertimeId { get; set; }

        public Employee Employee { get; set; }

        public DateTime Date { get; set; }
        
        public float Hour { get; set; }

        public float NexHour { get; set; }

        public float AdjustHour { get; set; }

        public string Note { get; set; }

        public string Unique { get; set; }
    }
}
