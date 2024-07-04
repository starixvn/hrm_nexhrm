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
        public string Unique { get; set; }

        public string DisplayName => NickName + (string.IsNullOrWhiteSpace(FullName) ? string.Empty : " (" + FullName + ")");

    }
}
