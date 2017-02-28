namespace Fanex.NexHR.ViewModels.Report
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class RequestsMonitoringViewModel
    {
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }

        [Display(Name = "Type")]
        public int RequestType { get; set; }

        [Display(Name = "Times >=")]
        public int MaxRequest { get; set; }

        public int DateOption { get; set; }

        public bool IsLeader { get; set; }
    }
}