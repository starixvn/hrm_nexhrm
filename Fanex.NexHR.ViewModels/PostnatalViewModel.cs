using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fanex.NexHR.ViewModels
{
    public class PostnatalViewModel
    {
        public int EmpId { get; set; }

        public string Nickname { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int OldEmpId { get; set; }

        public DateTime OldFromDate { get; set; }

        public DateTime OldToDate { get; set; }

        public string ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int Minutes { get; set; }

        public int Type { get; set; }        
    }
}
