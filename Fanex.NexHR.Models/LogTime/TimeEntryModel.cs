namespace Fanex.NexHR.Models.LogTime
{
    using System;

    public class TimeEntryModel
    {
        public int UserId { get; set; }
        public string Login { get; set; }
        public DateTime SpentOn { get; set; }
        public double Hours { get; set; }
    }
}