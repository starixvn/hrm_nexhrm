using Fanex.NexHR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Fanex.NexHR.Resources;
using Fanex.NexHR.Core.Enums;

namespace Fanex.NexHR.ViewModels
{
    public class OverTimeViewModel
    {
        [Display(Name = "Date", ResourceType = typeof(Labels))]
        [Required]
        public DateTime Date { get; set; }

        [Display(Name = "Hours", ResourceType = typeof(Labels))]
        public int Hour { get; set; }
        public int CreatedBy { get; set; }
        public int ApprovedBy { get; set; }
        public int VerifiedBy { get; set; }

        [Display(Name = "Description", ResourceType = typeof(Labels))]
        [Required]
        public string Description { get; set; }
        public int Status { get; set; }
        public List<OverTimeRecord> Records { get; set; }
        public string RecordData { get; set; }

        public OverTimeViewModel()
        {
            Date = DateTime.Now;
            Hour = 2;
            Status = (int)OTRequestStatus.New;
            Records = new List<OverTimeRecord>();
        }
    }
}
