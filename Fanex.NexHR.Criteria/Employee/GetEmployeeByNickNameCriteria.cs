namespace Fanex.NexHR.Criteria.Employee
{
    using Data.Repository;

    public class GetEmployeeByNickNameCriteria : CriteriaBase
    {
        public string NickName { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(NickName);
        }

        public override string GetSettingKey()
        {
            return "Employee_GetByNickName";
        }
    }
}
