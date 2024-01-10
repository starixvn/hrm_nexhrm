namespace Fanex.NexHR.Models
{
    using System;

    public class Transfer
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public string TeamName { get; set; }

        public DateTime AffectedDate { get; set; }
    }
}
