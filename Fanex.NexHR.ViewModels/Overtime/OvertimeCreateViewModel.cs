namespace Fanex.NexHR.ViewModels.Overtime
{
    using Core.Enums;
    using Models.Overtime;
    using Resources;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CreateOvertimeViewModel
    {
        private const int length = 4000;

        public CreateOvertimeViewModel()
        {
            Date = DateTime.Now;
            Status = (int)RequestStatuse.Pending;
            TimeSheets = new List<OvertimeRecord>();
        }

        public int Id { get; set; }
        
        public DateTime Date { get; set; }

        [Display(Name = "Description", ResourceType = typeof(Labels))]
        [Required]
        [StringLength(length)]
        public string Description { get; set; }

        public string CreatedByName { get; set; }

        public int Status { get; set; }

        [Required(ErrorMessageResourceName = "RequiredEmployee", ErrorMessageResourceType = typeof(Messages))]
        public string TimeSheetData { get; set; }

        public IList<OvertimeRecord> TimeSheets { get; set; }

    }
}
