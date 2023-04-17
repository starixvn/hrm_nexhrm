using System;

namespace Fanex.NexHR.Criteria.EmployeeExperience
{
    public class InsertEmployeeExperienceCriteria : BaseCommand
    {
        public int EmpId { get; set; }
        public int DepartmentId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Organization { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ExamDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public override string GetSettingKey() => "EmployeeExperience_Insert";

        public override bool IsValid() => EmpId > 0;
    }
}
