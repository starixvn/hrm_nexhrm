namespace Fanex.NexHR.Criteria.Bio
{
    using Fanex.Data.Repository;

    public class GetBioDevicesCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Bio_GetBioDevices";

        public override bool IsValid() => true;
    }
}