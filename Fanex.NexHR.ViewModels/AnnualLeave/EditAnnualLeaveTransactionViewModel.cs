namespace Fanex.NexHR.ViewModels.AnnualLeave
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class EditAnnualLeaveTransactionViewModel
    {
        [Required]
        public int LeaveTransactionId { get; set; }

        [Required]
        public decimal TotalDays { get; set; }

        public string Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Affect From")]
        public DateTime AffectedFrom { get; set; }

        public int EmployeeId { get; set; }
        public bool ExcludedRequestTrans { get; set; }
    }
}
