namespace Fanex.NexHR.Criteria.Bio
{
    using Core.Enums;
    using Data.Repository;

    public class UpdateFingerprintStatusCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public int BioUserId { get; set; }

        public int FingerIndex { get; set; }

        public int UpdatedBy { get; set; }

        public FingerprintScannerAction Status { get; set; }

        public override string GetSettingKey() => "Bio_UpdateFingerprintStatus";

        public override bool IsValid() => EmployeeId > 0
                                            && BioUserId > 0
                                            && FingerIndex > 0
                                            && UpdatedBy > 0;
    }
}