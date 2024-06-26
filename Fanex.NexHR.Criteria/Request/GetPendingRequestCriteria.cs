namespace Fanex.NexHR.Criteria.Request
{
    using Fanex.Data.Repository;

    public class GetPendingRequestCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "WorkingTimeRequest_GetPendingRequest";

        public override bool IsValid() => true;
    }
}