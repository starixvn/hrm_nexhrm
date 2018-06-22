namespace Fanex.NexHR.Criteria.Bio
{
    using Fanex.Data.Repository;

    public class EditBioDeviceCriteria : CriteriaBase
    {
        public int DeviceId { get; set; }

        public int? StatusId { get; set; }

        public int? DisplayId { get; set; }

        public int UpdatedBy { get; set; }

        public override string GetSettingKey() => "Bio_EditBioDevice";

        public override bool IsValid() => DeviceId > 0 && UpdatedBy > 0;
    }
}