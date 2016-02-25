namespace Fanex.NexHR.Models
{
    using System;

    public class OvertimeRecord
    {
        public int Id { get; set; }

        public Employee Employee { get; set; }

        public DateTime OnDate { get; set; }

        public float Hour { get; set; }
    }
}
