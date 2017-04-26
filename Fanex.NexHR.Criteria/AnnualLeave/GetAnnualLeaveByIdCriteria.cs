namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using Data.Repository;

    public class GetAnnualLeaveByIdCriteria : CriteriaBase
    {
        public int LeaveTransactionId { get; set; }

        public override string GetSettingKey()
        {
            return "AnnualLeave_GetByID";
        }

        public override bool IsValid()
        {
            return LeaveTransactionId > 0;
        }
    }
}
