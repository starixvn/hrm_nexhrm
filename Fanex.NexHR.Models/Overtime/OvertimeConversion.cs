namespace Fanex.NexHR.Models.Overtime
{
    using System;

    public class OvertimeConversion
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string TeamCode { get; set; }

        public string TeamName { get; set; }

        public float Deduct { get; set; }

        public DateTime DeductDate { get; set; }

        public string Description { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
