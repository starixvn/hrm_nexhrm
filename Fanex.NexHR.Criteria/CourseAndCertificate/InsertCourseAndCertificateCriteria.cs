using System;

namespace Fanex.NexHR.Criteria.CourseAndCertificate
{
    public class InsertCourseAndCertificateCriteria : BaseCommand
    {
        public int EmpId { get; set; }
        public int CreatedBy { get; set; }
        public string Department { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Organization { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ExamDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public override string GetSettingKey() => "CourseAndCertificate_Insert";

        public override bool IsValid() => EmpId > 0;
    }
}
