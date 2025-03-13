using System;

namespace Fanex.NexHR.Models
{
    public class EmployeeManagementGrade
    {
        public int Id { get; set; }
        public int EmpId { get; set; }
        public int GroupId { get; set; }
        public string NickName { get; set; }
        public string FullName { get; set; }
        public string TeamName { get; set; }
        public string TeamCode { get; set; }

        public int ManagementGradeId { get; set; }

        public string ManagementGradeCode { get; set; }

        public string ManagementGradeName { get; set; }

        public DateTime? AffectedDate { get; set; }

        public string Note { get; set; } 

    }
}
