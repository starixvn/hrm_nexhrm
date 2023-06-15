namespace Fanex.NexHR.Criteria.TrainingAndDevelopment
{
    using Fanex.Data.Repository;

    public class GetTrainingAndDevelopmentByEmpIdCriteria : CriteriaBase
    {
        public int EmpId { get; set; }
        public int CurrentUserId { get; set; }

        public override string GetSettingKey() => "TrainingAndDevelopment_GetByEmpId";

        public override bool IsValid() => EmpId > 0 && CurrentUserId > 0;
    }
}
