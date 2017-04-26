namespace Fanex.NexHR.Models
{
    using System;

    public class Postnatal
    {
        public int EmpId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int ModifiedBy { get; set; }

        public int Minutes { get; set; }
    }
}
