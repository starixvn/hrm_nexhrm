namespace Fanex.NexHR.Criteria.Bio
{
    public class EditBioDeviceCriteria : BaseCommand
    {
        public int DeviceId { get; set; }

        public string StatusId { get; set; }

        public string DisplayName { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "BioDevice_Edit";

        public override bool IsValid() => DeviceId > 0 && ModifiedBy > 0;
    }
}