namespace Fanex.NexHR.Models.Hangfire
{
    public class EmployeeConversation
    {
        public int EmpId { get; set; }

        public string FullName { get; set; }

        public string DomainUsername { get; set; }

        public string NickName { get; set; }

        public string ConversationId { get; set; }
    }
}