namespace Fanex.NexHR.ViewModels.Transfer
{
    using System;
    using Fanex.NexHR.Resources;
    using System.ComponentModel.DataAnnotations;

    public class CreateTransferViewModel
    {
        [Required]
        [Display(Name = "Employee", ResourceType = typeof(Labels))]
        public int EmployeeId { get; set; }

        public string Employee { get; set; }

        public int TeamId { get; set; }

        public DateTime TransferDate { get; set; }
    }
}
