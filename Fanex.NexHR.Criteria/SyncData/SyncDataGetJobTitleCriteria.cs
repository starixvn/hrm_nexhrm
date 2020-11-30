namespace Fanex.NexHR.Criteria.SyncData
{
    using Fanex.Data.Repository;

    public class SyncDataGetJobTitleCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "SyncData_GetJobTitle";

        public override bool IsValid() => true;
    }
}