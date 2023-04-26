namespace Fanex.NexHR.Criteria.CourseAndCertificate
{
    using System;

    public class UpdateCourseAndCertificateCriteria : BaseCommand
    {
        public int Id { get; set; }
        public string ConductedBy { get; set; }
        public int ModifyBy { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Organization { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ExamDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public override string GetSettingKey() => "CourseAndCertificate_Update";

        public override bool IsValid() => Id > 0;
    }
}
