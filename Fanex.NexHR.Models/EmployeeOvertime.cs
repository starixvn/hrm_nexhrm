namespace Fanex.NexHR.Models
{
    using System;

    public class EmployeeOvertime
    {
        public int OvertimeId { get; set; }

        public DateTime OnDate { get; set; }

        public string Description { get; set; }

        public float Hour { get; set; }

        public string Note { get; set; }

        public int Status { get; set; }

        public string CreatedBy { get; set; }

        public string EmployeeNickName { get; set; }

        public string EmployeeFullName { get; set; }
    }
}
