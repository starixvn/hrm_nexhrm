using System;

namespace Fanex.NexHR.Models.EmployeeExperience
{
    public class EmployeeExperience
    {
        public int Id { get; set; }
        public int EmpId { get; set; }
        public string NickName { get; set; }
        public string FullName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentCode { get; set; }
        public string TeamName { get; set; }
        public string EmployeeExperienceType { get; set; }
        public string EmployeeExperienceName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ExamDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Organization { get; set; }
    }
}
