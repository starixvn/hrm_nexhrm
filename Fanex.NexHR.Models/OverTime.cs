using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fanex.NexHR.Models
{
    public class OverTime
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public float Hour { get; set; }
        public int Status { get; set; }
        public Employee CreatedBy { get; set; }
        public Employee ApprovedBy { get; set; }
        public Employee VerifiedBy { get; set; }
        public Employee Employee { get; set; }
        
    }
}
