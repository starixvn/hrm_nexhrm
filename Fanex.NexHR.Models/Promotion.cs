namespace Fanex.NexHR.Models
{
    using System;

    public class Promotion
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public string TeamCode { get; set; }

        public string TeamName { get; set; }

        public string CurrentGrade { get; set; }

        public string PromotedGrade { get; set; }

        public int OldJobGradeId { get; set; }

        public int JobGradeId { get; set; }

        public DateTime AffectedDate { get; set; }

        public string Note { get; set; }
    }
}
