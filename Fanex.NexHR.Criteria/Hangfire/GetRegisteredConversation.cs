namespace Fanex.NexHR.Criteria.Hangfire
{
    using Core.Enums;
    using Fanex.Data.Repository;

    public class GetRegisteredConversation : CriteriaBase
    {
        public HangfireCategoryMessageType CategoryId { get; set; }

        public override string GetSettingKey() => "NexBot_GetRegisteredConversation";

        public override bool IsValid() => CategoryId > 0;
    }
}