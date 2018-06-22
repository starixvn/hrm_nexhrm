namespace Fanex.NexHR.Criteria.Bio
{
    using Fanex.Data.Repository;

    public class GetBioDeviceStatusesCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Bio_GetBioDeviceStatuses";

        public override bool IsValid() => true;
    }
}