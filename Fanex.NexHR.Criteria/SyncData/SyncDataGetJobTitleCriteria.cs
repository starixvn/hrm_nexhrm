namespace Fanex.NexHR.Criteria.SyncData
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class SyncDataGetJobTitleCriteria : CriteriaBase
    {
        [SpParam(Ignored = true)]
        public bool IsNexHRM { get; set; }

        public override string GetSettingKey() => IsNexHRM ? "SyncData_NexHRM_GetJobTitle" : "SyncData_LumiHRM_GetJobTitle";

        public override bool IsValid() => true;
    }
}