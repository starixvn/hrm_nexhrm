namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using System;
    using Data;
    using Data.Repository;

    public class GetLeaveConversionDetailCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int? GroupId { get; set; }

        public int? EmployeeId { get; set; }

        [SpParam(SqlParamName = "Type")]
        public int ConversionType { get; set; }

        public override string GetSettingKey() => "LeaveConversion_Get";

        public override bool IsValid() => FromDate <= ToDate;
    }
}
