namespace Fanex.NexHR.Criteria.Request
{
    using Data.Repository;
    using System;

    public class GetLeaveBalanceInMonthCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public long? RequestId { get; set; }

        public DateTime EffectedDate { get; set; }

        public override bool IsValid()
        {
            return EmpId > 0;
        }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_AvaibleLeaveBalance";
        }
    }
}
