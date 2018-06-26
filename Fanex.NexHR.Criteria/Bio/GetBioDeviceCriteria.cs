namespace Fanex.NexHR.Criteria.Bio
{
    using Fanex.Data.Repository;

    public class GetBioDeviceCriteria : CriteriaBase
    {
        public int DeviceId { get; set; }

        public override string GetSettingKey() => "BioDevice_Get";

        public override bool IsValid() => DeviceId > 0;
    }
}