namespace Fanex.NexHR.Criteria.Bio
{
    using Fanex.Data.Repository;

    public class GetBioDeviceDisplayNamesCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Bio_GetBioDeviceDisplayNames";

        public override bool IsValid() => true;
    }
}