namespace Fanex.NexHR.Criteria.Bio
{
    using Data.Repository;

    public class GetFingerprintDetailByEmployeeIdCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public override string GetSettingKey() => "BioFingerprint_Get";

        public override bool IsValid() => EmpId > 0;
    }
}