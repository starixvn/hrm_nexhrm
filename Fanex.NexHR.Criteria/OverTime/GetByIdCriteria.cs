namespace Fanex.NexHR.Criteria.Overtime
{
    using Fanex.Data.Repository;

    public class GetByIdCriteria : CriteriaBase
    {        
        public int Id { get; set; }     

        public override string GetSettingKey()
        {
            return "Overtime_GetById";
        }
    }
}
