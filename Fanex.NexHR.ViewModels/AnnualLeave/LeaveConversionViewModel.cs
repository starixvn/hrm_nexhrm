namespace Fanex.NexHR.ViewModels.AnnualLeave
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Resources;

    public class LeaveConversionViewModel
    {
        public int LeaveConversionId { get; set; }

        public int EmpId { get; set; }

        [Display(Name = "Date", ResourceType = typeof(Labels))]
        public DateTime OnDate { get; set; }

        [Display(Name = "TotalDays", ResourceType = typeof(Labels))]
        public decimal TotalDays { get; set; }

        public string Description { get; set; }
    }
}
