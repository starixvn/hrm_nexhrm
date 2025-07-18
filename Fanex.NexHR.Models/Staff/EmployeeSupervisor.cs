namespace Fanex.NexHR.Models.Staff
{
    public class EmployeeSupervisor
    {
        public int EmpId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string Code { get; set; }

        public int SupervisorId { get; set; }

        public string SupervisorNickName { get; set; }

        public string SupervisorFullName { get; set; }

        public string SupervisorEmail { get; set; }

        public string SupervisorConversationId { get; set; }

        public string SupervisorLetstalkId { get; set; }

        public string PICDisplayName { get; set; }

        public bool IsReadOnly { get; set; }
    }
}
