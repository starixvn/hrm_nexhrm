namespace Fanex.NexHR.Criteria.Employee
{
    public class MappingEmployeeConversationCommand : BaseCommand
    {
        public string Username { get; set; }

        public string ConversationId { get; set; }

        public override string GetSettingKey() => "EmployeeConversation_MappingV2";

        public override bool IsValid() => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(ConversationId);
    }
}
