using Fanex.NexHR.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Fanex.NexHR.ViewModels
{
    public class OverTimeSummaryFilterViewModel
    {
        [DataType(DataType.Date)]
        [Display(Name = "FromDate", ResourceType = typeof(Labels))]
        public DateTime FromDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "ToDate", ResourceType = typeof(Labels))]
        public DateTime ToDate { get; set; }


        [Display(Name = "CreateAccount", ResourceType = typeof(Labels))]
        public int CreatedBy { get; set; }

        [Display(Name = "Employee", ResourceType = typeof(Labels))]
        public int EmployeeId { get; set; }

        [Display(Name = "Status", ResourceType = typeof(Labels))]
        public int Status { get; set; }

    }
}
