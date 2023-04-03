namespace Fanex.NexHR.Criteria.ScheduledMessage
{
    using Fanex.Data.Repository;

    public class GetTeamHrConversationCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Conversation_GetTeamHr";

        public override bool IsValid() => true;
    }
}
