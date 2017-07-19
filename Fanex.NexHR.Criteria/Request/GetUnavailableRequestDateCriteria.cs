namespace Fanex.NexHR.Criteria.Request
{
    using System;
    using Data;
    using Data.Repository;

    public class GetUnavailableRequestDateCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpId")]
        public int EmployeeId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeCalendar_GetUnavailRequestDate";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0 && FromDate <= ToDate;
        }
    }
}
