namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using Data.Repository;

    public class GetAnnualLeaveByIdCriteria : CriteriaBase
    {
        public int LeaveTransactionId { get; set; }

        public override string GetSettingKey() => "AnnualLeave_GetByID";

        public override bool IsValid() => LeaveTransactionId > 0;
    }
}
