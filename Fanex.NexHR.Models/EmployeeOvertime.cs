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

        public int EmployeeId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }
    }
}
