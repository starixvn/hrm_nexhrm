namespace Fanex.NexHR.Criteria.TrainingAndDevelopment
{
    using Fanex.Data.Repository;
    using System;

    public class GetTrainingAndDevelopmentCriteria : CriteriaBase
    {
        public int? GroupId { get; set; }

        public int? EmpId { get; set; }

        public int? TypeId { get; set; }

        public string Name { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "TrainingAndDevelopment_Get";

        public override bool IsValid() => ToDate >= FromDate;
    }
}
