namespace Fanex.NexHR.Criteria.History
{
    using Fanex.Data.Repository;

    public class GetHistoryByEmpIdCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public override string GetSettingKey() => "History_GetByEmpId";

        public override bool IsValid() => EmpId > 0;
    }
}
