namespace Fanex.NexHR.Models.Overtime
{
    using System;

    public class OvertimeConversion
    {
        public int Id { get; set; }

        public Employee Employee { get; set; }

        public float Deduct { get; set; }

        public DateTime DeductDate { get; set; }

        public string Description { get; set; }

        public Employee ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
