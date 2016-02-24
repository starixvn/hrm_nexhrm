namespace Fanex.NexHR.Models
{
    using System;

    public class Holiday : EntityBase<int>
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }
}
