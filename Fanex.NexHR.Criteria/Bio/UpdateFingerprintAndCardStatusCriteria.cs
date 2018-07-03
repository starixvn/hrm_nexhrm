namespace Fanex.NexHR.Criteria.Bio
{
    using Core.Enums;
    using Data.Repository;

    public class UpdateFingerprintAndCardStatusCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public int BioUserId { get; set; }

        public int UpdatedBy { get; set; }

        public FingerprintScannerStatus Status { get; set; }

        public override string GetSettingKey() => "Bio_UpdateFingerprintAndCardStatus";

        public override bool IsValid() => EmployeeId > 0 && UpdatedBy > 0 && BioUserId > 0;
    }
}