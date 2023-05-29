namespace Fanex.NexHR.Criteria.TrainingAndDevelopment
{
    using System;

    public class UpdateTrainingAndDevelopmentCriteria : BaseCommand
    {
        public int? Id { get; set; }
        public string ConductedBy { get; set; }
        public int? ModifyBy { get; set; }
        public int? TypeId { get; set; }
        public string Name { get; set; }
        public string Organization { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ExamDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public override string GetSettingKey() => "TrainingAndDevelopment_Update";

        public override bool IsValid() => IsvalidId() && IsvalidContent() && (IsvalidDateTime(StartDate, EndDate)
            || IsvalidDateTime(ExamDate, ExpirationDate) || IsvalidDateTimeCertificate(ExamDate, ExpirationDate));
        private bool IsvalidId() => Id > 0 && TypeId > 0;
        private bool IsvalidContent() => !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Organization);
        private static bool IsvalidDateTime(DateTime? fromdate, DateTime? todate) => fromdate.HasValue && todate.HasValue && todate >= fromdate;
        private static bool IsvalidDateTimeCertificate(DateTime? fromdate, DateTime? todate) => !todate.HasValue && fromdate.HasValue;
    }
}
