namespace Fanex.NexHR.Models
{
    using System;

    public class History
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public string GroupName { get; set; }

        public string JobGradeName { get; set; }

        public string JobTitleName { get; set; }

        public DateTime AffectedDate { get; set; }
    }
}
