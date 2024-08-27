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

        public int CreatedById { get; set; }

        public string CreatedByNickName { get; set; }

        public string CreatedByFullName { get; set; }

        public string CreatedByDisplayName 
        {
            get 
            { 
                return CreatedByNickName + " (" + CreatedByFullName + ")";
            }
        } 

        public DateTime CreatedDate { get; set; }

        public int TotalMember => OvertimeRecords.Count();

        public float Totalhours
        {
            get
            {
                return OvertimeRecords.Sum(t => t.Hour);
            }
        }

        public IEnumerable<OvertimeRecord> OvertimeRecords { get; set; }

        public bool Own { get; set; }
    }
}
