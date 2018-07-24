namespace Fanex.NexHR.Criteria.Hangfire
{
    using Core.Enums;
    using Fanex.Data.Repository;

    public class GetAllEmployeeConversationCriteria : CriteriaBase
    {
        public HangfireCategoryMessageType CategoryId { get; set; }

        public override string GetSettingKey() => "NexBot_GetAllEmployeeConversation";

        public override bool IsValid() => CategoryId > 0;
    }
}