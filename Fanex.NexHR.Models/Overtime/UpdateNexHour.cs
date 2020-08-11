namespace Fanex.NexHR.Models.Overtime
{
    using System;

    public class UpdateNexHour
    {
        public int EmployeeId { get; set; }

        public int OvertimeId { get; set; }

        public int Hour { get; set; }

        public DateTime Date { get; set; }

        public float AdjustHour { get; set; }

        public string Description { get; set; }
    }
}
