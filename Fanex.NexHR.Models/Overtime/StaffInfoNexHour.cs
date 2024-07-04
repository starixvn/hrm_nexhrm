namespace Fanex.NexHR.Models.Overtime
{
    using System;

    public class StaffInfoNexHour : StaffInfo
    {
        public float NexHour { get; set; }

        public bool IsWorkDay { get; set; }

        public float Hour { get; set; }
        public DateTime Date { get; set; }
        public string Note { get; set; }
        public DateTime RunTime { get; set; }

        public string Unique => RunTime.Subtract(DateTime.UnixEpoch).TotalMicroseconds.ToString();

        public string DisplayName => NickName + (string.IsNullOrWhiteSpace(FullName) ? string.Empty : " (" + FullName + ")");

    }
}
