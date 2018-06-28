namespace Fanex.NexHR.Criteria.Bio
{
    using Core.Enums;
    using Fanex.Data.Repository;

    public class UpdateCardStatusCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public int CardId { get; set; }

        public int UpdatedBy { get; set; }

        public FingerprintScannerAction Status { get; set; }

        public override string GetSettingKey() => "Bio_UpdateCardStatus";

        public override bool IsValid() => EmployeeId > 0 && CardId > 0 && UpdatedBy > 0;
    }
}