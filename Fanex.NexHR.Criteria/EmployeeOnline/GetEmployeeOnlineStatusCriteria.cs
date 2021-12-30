namespace Fanex.NexHR.Criteria.EmployeeOnline
{
    using Data.Repository;

    public class GetEmployeeOnlineStatusCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "EmployeeOnline_GetAllStatus";

        public override bool IsValid() => true;
    }
}
