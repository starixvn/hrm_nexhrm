namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetMemberByManagerCriteria : CriteriaBase
    {
        public int ManagerId { get; set; }

        public int Role { get; set; }

        public bool ExcludedManager { get; set; }

        public int? GroupId { get; set; }

        public string Keyword { get; set; }

        public override string GetSettingKey()
        {
            return "Employee_GetMembesByManager";
        }

        public override bool IsValid()
        {
            return ManagerId > 0;
        }
    }
}
