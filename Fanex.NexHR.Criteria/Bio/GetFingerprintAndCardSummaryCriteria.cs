namespace Fanex.NexHR.Criteria.Bio
{
    using Core.Enums;
    using Data.Repository;

    public class GetFingerprintAndCardSummaryCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public int? GroupId { get; set; }

        public FingerprintScannerType? Type { get; set; }

        public override string GetSettingKey() => "Bio_GetFigerprintAndCard";

        public override bool IsValid() => true;
    }
}