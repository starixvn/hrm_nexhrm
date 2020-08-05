namespace Fanex.NexHR.Models.Overtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Overtime
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public int Status { get; set; }

        public Employee CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public Employee ModifiedBy { get; set; }

        public int TotalMember => OvertimeRecords.Count;

        public float Totalhours
        {
            get
            {
                return OvertimeRecords.Sum(t => t.Hour);
            }
        }

        public IList<OvertimeRecord> OvertimeRecords { get; set; }
    }
}
