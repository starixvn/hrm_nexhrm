namespace Fanex.NexHR.ViewModels.AnnualLeave
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Resources;

    public class LeaveDeductionViewModel
    {
        public int LeaveDeductionId { get; set; }

        public int EmpId { get; set; }

        [Display(Name = "DeductDate", ResourceType = typeof(Labels))]
        public DateTime DeductDate { get; set; }

        [Display(Name = "IsAnnual", ResourceType = typeof(Labels))]
        public bool IsAnnual { get; set; }

        [Display(Name = "TotalDays", ResourceType = typeof(Labels))]
        public decimal TotalDays { get; set; }

        public string Description { get; set; }
    }
}
