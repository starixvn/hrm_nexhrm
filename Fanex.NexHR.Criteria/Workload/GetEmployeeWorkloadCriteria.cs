namespace Fanex.NexHR.Criteria.Workload
{
    using System;
    using Data;
    using Data.Repository;

    public class GetEmployeeWorkloadCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        [SpParam(SqlParamName = "GroupId")]
        public int TeamId { get; set; }

        public override bool IsValid()
        {
            return TeamId != 0;
        }

        public override string GetSettingKey()
        {
            return "Employee_GetEmployeeWorkLoad";
        }
    }
}
