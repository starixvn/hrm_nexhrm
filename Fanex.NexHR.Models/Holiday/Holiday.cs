namespace Fanex.NexHR.Models.Holiday
{
    using System;

    public class Holiday : EntityBase<int>
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }
}
