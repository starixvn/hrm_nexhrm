namespace Fanex.NexHR.Models
{
    using System;

    public class EmployeeWorkFromHome
    {
        public int EmployeeWorkFromHomeId { get; set; }

        public int EmpId { get; set; }

        public string Nickname { get; set; }

        public string Fullname { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string Remark { get; set; }

        public byte StatusId { get; set; }

        public string Status { get; set; }

        public string ProcessedBy { get; set; }

        public DateTime ProcessedDate { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
