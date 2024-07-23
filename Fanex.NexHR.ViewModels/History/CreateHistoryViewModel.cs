namespace Fanex.NexHR.ViewModels.History
{
    using System;
    using Fanex.NexHR.Resources;
    using System.ComponentModel.DataAnnotations;

    public class CreateHistoryViewModel
    {
        [Required]
        [Display(Name = "Employee", ResourceType = typeof(Labels))]
        public int EmployeeId { get; set; }

        public string Employee { get; set; }

        public int GroupId { get; set; }

        public DateTime TransferDate { get; set; }
    }
}
