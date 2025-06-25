namespace Fanex.NexHR.Models.Overtime
{
    using System;
    using Fanex.NexHR.Core;

    public class OvertimeRecord
    {
        public OvertimeRecord()
        {
            Date = DateTimeOffset.Now.DateTime.ToClientTimeZone();
            Hour = 1;
        }

        public int OvertimeId { get; set; }

        public int EmployeeId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public DateTime Date { get; set; }

        public float Hour { get; set; }

        public float NexHour { get; set; }

        public string Note { get; set; }

        public string Unique { get; set; }
    }
}
