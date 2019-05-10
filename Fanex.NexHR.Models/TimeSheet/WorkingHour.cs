using Fanex.NexHR.Resources;

namespace Fanex.NexHR.Models.TimeSheet
{
    public class WorkingHour
    {
        public int EmployeeId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public float TotalWorkHours { get; set; }

        public string SkypeConversationId { get; set; }

        public string MessageContent
        {
            get
            {
                return string.Format(Messages.RemindReachedWorkingTime, NickName, TotalWorkHours);
            }
        }
    }
}
