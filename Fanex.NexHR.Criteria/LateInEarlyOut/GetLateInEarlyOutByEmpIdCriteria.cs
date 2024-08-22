namespace Fanex.NexHR.Criteria.LateInEarlyOut
{
    using Data.Repository;

    public class GetLateInEarlyOutByEmpIdCriteria : CriteriaBase
    {
        public int EmpId { get; set; }        

        public int Year { get; set; }

        public override bool IsValid() => EmpId > 0 && Year > 0;

        public override string GetSettingKey() => "LateInEarlyOut_GetAllByEmpId";
    }
}
