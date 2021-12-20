namespace Fanex.NexHR.Criteria.EmployeeWorkFromHome
{
    using System;
    using Fanex.Data.Repository;

    public class GetEmployeeWorkFromHomeCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int? GroupId { get; set; }

        public int? EmpId { get; set; }


        public override string GetSettingKey()
        {
            return "EmployeeWorkFromHome_Get";
        }

        public override bool IsValid()
        {
            return ToDate >= FromDate;
        }
    }
}
