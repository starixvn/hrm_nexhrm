namespace Fanex.NexHR.Criteria.Bio
{
    using Fanex.Data.Repository;

    public class GetPendingBioDevicesCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Bio_GetPendingDevice";

        public override bool IsValid() => true;
    }
}
