namespace Fanex.NexHR.Criteria.RedmineAPI
{
    using Fanex.Data.Repository;

    public class GetRedmineUserCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "HRM_GetRedmineUsers";

        public override bool IsValid() => true;
    }
}
