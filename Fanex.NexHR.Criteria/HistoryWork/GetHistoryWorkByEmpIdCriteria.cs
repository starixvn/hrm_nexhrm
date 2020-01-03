namespace Fanex.NexHR.Criteria.HistoryWork
{
    using Data.Repository;

    public class GetHistoryWorkByEmpIdCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public override string GetSettingKey()
        {
            return "HistoryWork_GetByEmpId";
        }

        public override bool IsValid()
        {
            return EmpId > 0;
        }
    }   
}
