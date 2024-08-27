namespace Fanex.NexHR.Criteria.Overtime
{
    using Data.Repository;

    public class GetOvertimeByIdCriteria : CriteriaBase
    {        
        public int Id { get; set; }

        public override string GetSettingKey() => "Overtime_GetById";

        public override bool IsValid() => Id > 0;
    }
}
