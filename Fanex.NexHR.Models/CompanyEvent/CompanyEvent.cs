namespace Fanex.NexHR.Models.CompanyEvent
{
    using System;

    public class CompanyEvent
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public TimeSpan FromTime { get; set; }

        public TimeSpan ToTime { get; set; }

        public string Description { get; set; }
    }
}
