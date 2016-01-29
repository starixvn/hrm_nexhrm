namespace Fanex.NexHR.Criteria.OverTime
{
    using Fanex.Data.Repository;

    public class GetByIdCriteria : CriteriaBase
    {        
        public int Id { get; set; }     

        public override string GetSettingKey()
        {
            return "OverTime_GetById";
        }
    }
}
