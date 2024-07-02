namespace Fanex.NexHR.Models.TimeSheet
{
    using System;
    using Fanex.NexHR.Core;

    public class CheckInCheckOutNotifyRecord
    {
        public int EmployeeId { get; set; }
        public string Nickname { get; set; }
        public string ConversationId { get; set; }
        public DateTime CheckOutTime { get; set; }
        public DateTime? CheckInTime { get; set; }
        public float TotalWorkHours { get; set; }

        public string MessageContent =>
            CheckInTime is not null && CheckInTime.HasValue ?
            string.Format(Resources.Labels.NotifyCheckInMessageContent, Nickname, CheckInTime.Value.ToString("hh:mm tt")) :
            string.Format(Resources.Labels.NotifyCheckOutMessageContent,
                Nickname,
                CheckOutTime.ToString("hh:mm tt"),
                Utilities.TotalHoursToTime(TotalWorkHours)
                );
    }
}