namespace Fanex.NexHR.Criteria.Bio
{
    using Fanex.Data.Repository;

    public class GetCardDetailByEmployeeIdCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public override string GetSettingKey() => "BioCard_Get";

        public override bool IsValid() => EmpId > 0;
    }
}