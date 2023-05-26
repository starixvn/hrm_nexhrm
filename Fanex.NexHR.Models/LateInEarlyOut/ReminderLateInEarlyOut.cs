using System;

namespace Fanex.NexHR.Models.LateInEarlyOut
{
    public class ReminderLateInEarlyOut
    {
        public int EmpId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public DateTime AttendanceDate { get; set; }

        public string ConversationId { get; set; }
    }
}
