namespace Fanex.NexHR.Criteria.SyncData
{
    using Fanex.Data.Repository;

    public class GetUnitHeadInfoCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Sync_GO_UnitHead_GetInfo";

        public override bool IsValid() => true;
    }
}