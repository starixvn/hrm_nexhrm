namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetCurrentUserSettingCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "EmployeeSetting_Get";

        public override bool IsValid() => EmployeeId > 0;
    }
}