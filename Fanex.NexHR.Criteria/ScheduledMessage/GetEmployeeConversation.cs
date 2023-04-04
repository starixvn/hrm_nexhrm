namespace Fanex.NexHR.Criteria.ScheduledMessage
{
    using Fanex.Data.Repository;

    public class GetEmployeeConversation : CriteriaBase
    {
        public override string GetSettingKey() => "EmployeeConversation_Get";

        public override bool IsValid() => true;
    }
}
