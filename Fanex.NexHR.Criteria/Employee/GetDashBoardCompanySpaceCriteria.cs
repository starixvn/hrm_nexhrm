namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Fanex.Data.Repository;

    public class GetDashBoardCompanySpaceCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public override bool IsValid()
        {
            return EmpId > 0;
        }

        public override string GetSettingKey()
        {
            return "Report_CompanySpaceDashBoard";
        }
    }
}
