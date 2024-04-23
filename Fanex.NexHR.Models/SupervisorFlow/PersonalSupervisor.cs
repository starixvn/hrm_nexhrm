using System;

namespace Fanex.NexHR.Models
{
    public class PersonalSupervisor
    {
        public int Id { get; set; }
        public int EmpId { get; set; }
        public int GroupId { get; set; }
        public string NickName { get; set; }
        public string FullName { get; set; }
        public string TeamName { get; set; }
        public string TeamCode { get; set; }

        public int JobGradeId { get; set; }

        public string JobGradeCode { get; set; }

        public string JobGradeName { get; set; }

        public DateTime? AffectedDate { get; set; }

        public string Note { get; set; } 

    }
}
