namespace Fanex.NexHR.Models.RedmineAPI
{
    using System;
    using Core.Enums;

    public class TimeEntry
    {
        public double Hours { get; set; }

        public DateTime SpentOn { get; set; }

        public RedmineLogTimeActivity Activity { get; set; }
    }
}