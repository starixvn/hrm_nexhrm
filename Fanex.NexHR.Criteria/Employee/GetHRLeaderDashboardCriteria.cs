namespace Fanex.NexHR.Criteria.Employee
{
    using Data.Repository;

    public class GetHRLeaderDashboardCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpId")]
        public int EmployeeId { get; set; }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }

        public override string GetSettingKey()
        {
            return "Report_HRLeaderDashBoard";
        }
    }
}
