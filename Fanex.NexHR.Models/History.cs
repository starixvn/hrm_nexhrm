namespace Fanex.NexHR.Models
{
    using System;

    public class History
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public int CurrentJobTitleId { get; set; }

        public int CurrentGroupId { get; set; }

        public int CurrentJobGradeId { get; set; }

        public int? GroupId { get; set; }

        public string GroupName { get; set; }

        public int? JobGradeId { get; set; }

        public string JobGradeName { get; set; }

        public int? JobTitleId { get; set; }

        public string JobTitleName { get; set; }

        public DateTime AffectedDate { get; set; }
    }
}
