namespace Fanex.NexHR.Models.Overtime
{
    using System;

    public class ReportOvertimeHRDetail
    {
        public int OvertimeId { get; set; }

        public string Note { get; set; }

        public DateTime Date { get; set; }

        public float Hour { get; set; }

        public float NexHour { get; set; }

        public int UserId { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public string DisplayName => NickName + (string.IsNullOrWhiteSpace(FullName) ? string.Empty : " (" + FullName + ")");
    }
}
