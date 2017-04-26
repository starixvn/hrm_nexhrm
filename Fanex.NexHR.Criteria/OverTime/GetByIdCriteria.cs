namespace Fanex.NexHR.Criteria.Overtime
{
    using Data.Repository;

    public class GetOvertimeByIdCriteria : CriteriaBase
    {        
        public int Id { get; set; }

        public override string GetSettingKey()
        {
            return "Overtime_GetById";
        }

        public override bool IsValid()
        {
            return Id > 0;
        }
    }
}
