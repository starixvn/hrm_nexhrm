namespace Fanex.NexHR.Criteria.Promotion
{
    using Fanex.Data.Repository;

    public class GetJobGradeCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "JobGrade_GetAll";

        public override bool IsValid() => true;
    }
}
