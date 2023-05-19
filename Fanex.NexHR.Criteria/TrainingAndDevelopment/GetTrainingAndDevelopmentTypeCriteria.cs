namespace Fanex.NexHR.Criteria.TrainingAndDevelopment
{
    using Fanex.Data.Repository;

    public class GetTrainingAndDevelopmentTypeCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "TrainingAndDevelopmentType_Get";

        public override bool IsValid() => true;
    }
}
