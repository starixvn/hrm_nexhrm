using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Fanex.NexHR.Models
{
    public class LateInEarlyOut
    {
        public int Id { get; set; }

        public DateTime WorkDate { get; set; }

        public int Type { get; set; }

        public int Minutes { get; set; }

        public int EmpId { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
