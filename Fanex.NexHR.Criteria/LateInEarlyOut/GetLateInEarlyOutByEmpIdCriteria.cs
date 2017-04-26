namespace Fanex.NexHR.Criteria.LateInEarlyOut
{
    using Data.Repository;

    public class GetLateInEarlyOutByEmpIdCriteria : CriteriaBase
    {
        public int EmpId { get; set; }        

        public int Year { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "LateInEarlyOut_GetAllByEmpId";
        }
    }
}
