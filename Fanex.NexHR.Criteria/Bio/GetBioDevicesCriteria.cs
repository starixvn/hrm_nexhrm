namespace Fanex.NexHR.Criteria.Bio
{
    using Fanex.Data.Repository;

    public class GetBioDevicesCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "BioDevice_GetAll";

        public override bool IsValid() => true;
    }
}