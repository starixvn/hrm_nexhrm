namespace Fanex.NexHR.Criteria.EmployeeExperience
{
    using System;

    public class UpdateEmployeeExperienceCriteria : BaseCommand
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public int ModifyBy { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Organization { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ExamDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public override string GetSettingKey() => "EmployeeExperience_Update";

        public override bool IsValid() => Id > 0;
    }
}
