namespace Fanex.NexHR.Criteria.ScheduledMessage
{
    using Fanex.Data.Repository;

    public class GetGroupConversationsCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "GroupConversation_Get";

        public override bool IsValid() => true;
    }
}