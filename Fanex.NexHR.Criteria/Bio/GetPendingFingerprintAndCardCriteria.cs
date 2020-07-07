namespace Fanex.NexHR.Models.Bio
{
    using Fanex.Data.Repository;

    public class GetPendingFingerprintAndCardCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Bio_GetPendingFingerprintAndCard";

        public override bool IsValid() => true;
    }
}