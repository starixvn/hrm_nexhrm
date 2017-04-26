namespace Fanex.NexHR.Criteria.Employee
{
    using Data.Repository;

    public class GetMySpaceCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public override bool IsValid()
        {
            return EmpId > 0;
        }

        public override string GetSettingKey()
        {
            return "Report_EmployeeDashBoard";
        }
    }
}
