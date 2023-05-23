namespace Fanex.NexHR.Criteria.TrainingAndDevelopment
{
    using System;

    public class InsertTrainingAndDevelopmentCriteria : BaseCommand
    {
        public int? EmpId { get; set; }
        public int CreatedBy { get; set; }
        public string ConductedBy { get; set; }
        public int? TypeId { get; set; }
        public string Name { get; set; }
        public string Organization { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ExamDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public override string GetSettingKey() => "TrainingAndDevelopment_Insert";

        public override bool IsValid() => EmpId > 0 && TypeId > 0 && !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Organization)
                                                    && Isvalid(StartDate, EndDate) || Isvalid(ExamDate, ExpirationDate);

        private static bool Isvalid(DateTime? fromdate, DateTime? todate) => fromdate.HasValue && todate.HasValue && todate >= fromdate;
    }
}
