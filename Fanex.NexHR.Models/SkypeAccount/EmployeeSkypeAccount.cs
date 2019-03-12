namespace Fanex.NexHR.Models.SkypeAccount
{
    public class EmployeeSkypeAccount
    {
        public Employee Employee { get; set; }

        public string SkypeDisplayName { get; set; }

        public string ConversationId { get; set; }

        public string ConversationNickName { get; set; }

        public bool IsConfirmed
        {
            get
            {
                return !string.IsNullOrWhiteSpace(ConversationId);
            }
        }
    }
}