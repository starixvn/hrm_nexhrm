namespace Fanex.NexHR.Models.Report
{
    using System;
    using System.Collections.Generic;

    public class HeadCountModel
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public IEnumerable<HeadCount> HeadCount { get; set; }
    }
}