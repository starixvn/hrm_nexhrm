namespace Fanex.NexHR.Criteria.Request
{
    using Data.Repository;
    public class GetRequestHistoryCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_GetRequestHistory";
        }

        public override bool IsValid()
        {
            return RequestId > 0;
        }
    }
}
