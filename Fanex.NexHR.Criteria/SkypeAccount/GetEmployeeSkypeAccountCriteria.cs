namespace Fanex.NexHR.Criteria.SkypeAccount
{
    using Fanex.Data.Repository;

    public class GetEmployeeSkypeAccountCriteria : CriteriaBase
    {
        public int? EmployeeId { get; set; }

        public int? GroupId { get; set; }

        public bool? IsConfirmed { get; set; }

        public override string GetSettingKey() => "Employee_GetSkypeAccount";

        public override bool IsValid() => true;
    }
}