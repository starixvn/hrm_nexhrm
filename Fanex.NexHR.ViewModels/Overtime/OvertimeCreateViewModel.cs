namespace Fanex.NexHR.ViewModels.Overtime
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Core.Enums;
    using Models.Overtime;
    using Resources;

    public class CreateOvertimeViewModel
    {
        public CreateOvertimeViewModel()
        {
            Date = DateTime.Now;
            Hour = 1;
            Status = (int)RequestStatuse.Pending;
            IsApprove = true;
            TimeSheets = new List<OvertimeRecord>();
        }

        public int Id { get; set; }

        [Display(Name = "Date", ResourceType = typeof(Labels))]
        [Required]
        public DateTime Date { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(Labels))]
        public string CreatedByName { get; set; }

        [Display(Name = "Approved", ResourceType = typeof(Labels))]
        public bool IsApprove { get; set; }

        [Display(Name = "Hours", ResourceType = typeof(Labels))]
        public float Hour { get; set; }

        [Display(Name = "Description", ResourceType = typeof(Labels))]
        [Required]
        [StringLength(4000)]
        public string Description { get; set; }

        public int Status { get; set; }

        [Required(ErrorMessageResourceName = "RequiredEmployee", ErrorMessageResourceType = typeof(Messages))]
        public string TimeSheetData { get; set; }

        public IList<OvertimeRecord> TimeSheets { get; set; }
    }
}
