namespace Fanex.NexHR.Criteria.Bio
{
    using Core.Enums;
    using Data;
    using Data.Repository;

    public class GetFingerprintAndCardSummaryCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public int? GroupId { get; set; }

        [SpParam(Ignored = true)]
        public FingerprintScannerType? Type { get; set; }

        public FingerprintScannerStatus? Status { get; set; }

        public override string GetSettingKey() => "Bio_GetFigerprintAndCardSummary";

        public override bool IsValid() => true;
    }
}