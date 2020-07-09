namespace Fanex.NexHR.Criteria.Hangfire
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core.Enums;

    public class GetJsonObjectByCategoryMessageCriteria : CriteriaBase
    {
        public MessageCategory CategoryId { get; set; }

        public override string GetSettingKey() => "NexHangfire_GetJsonObjectByCategoryId";

        public override bool IsValid() => CategoryId > 0;
    }
}