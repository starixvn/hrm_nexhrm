namespace Fanex.NexHR.Models
{
    using System;

    public class EmployeeOvertime
    {
        public int OvertimeId { get; set; }

        public DateTime OnDate { get; set; }

        public string Description { get; set; }

        public float Hour { get; set; }

        public int Status { get; set; }

        public Employee CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public Employee Employee { get; set; }
    }
}
