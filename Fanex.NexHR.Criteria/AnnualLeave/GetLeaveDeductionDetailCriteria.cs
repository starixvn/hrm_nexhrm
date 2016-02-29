namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using System;
    using Fanex.Data.Repository;

    public class GetLeaveDeductionDetailCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int? GroupId { get; set; }

        public int? EmployeeId { get; set; }

        public override string GetSettingKey()
        {
            return "LeaveDeduction_Get";
        }

        public override bool IsValid()
        {
            return FromDate <= ToDate;
        }
    }
}
