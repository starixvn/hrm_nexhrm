namespace Fanex.NexHR.ViewModels
{
    using Core.Enums;
    using Models;
    using Resources;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class OverTimeViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Date", ResourceType = typeof(Labels))]
        [Required]
        public DateTime Date { get; set; }

        [Display(Name = "Approved", ResourceType = typeof(Labels))]
        public bool IsApprove { get; set; }

        [Display(Name = "Hours", ResourceType = typeof(Labels))]
        public float Hour { get; set; }
        public int CreatedId { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(Labels))]
        public string CreatedNickName { get; set; }

        public int ModifiedId { get; set; }
        [Display(Name = "ModifiedBy", ResourceType = typeof(Labels))]
        public string ModifiedNickName { get; set; }

        public int EmployeeId { get; set; }
        public string EmployeeDisplayName { get; set; }

        [Display(Name = "Description", ResourceType = typeof(Labels))]
        [Required]        
        public string Description { get; set; }
        public int Status { get; set; }

        [Required(ErrorMessageResourceName = "RequiredEmployee", ErrorMessageResourceType = typeof(Messages))]
        public string RecordData { get; set; }

        public IList<OverTimeRecord> RecordList { get; set; }
        public OverTimeViewModel()
        {
            Date = DateTime.Now;
            Hour = 2;
            Status = (int)OTRequestStatus.Pending;
            IsApprove = true;
            RecordList = new List<OverTimeRecord>();
        }

        public static OverTimeViewModel Parse(OverTime overtime)
        {
            return new OverTimeViewModel()
            {
                Id = overtime.Id,
                Date = overtime.Date,
                Hour = overtime.Hour,
                EmployeeId = overtime.Employee.Id,
                EmployeeDisplayName = overtime.Employee.DisplayName,
                CreatedId = overtime.CreatedBy.Id,
                CreatedNickName = overtime.CreatedBy.NickName,
                ModifiedId = overtime.ModifiedBy != null ? overtime.ModifiedBy.Id : 0,
                ModifiedNickName = overtime.ModifiedBy != null ? overtime.ModifiedBy.NickName : string.Empty,
                Description = overtime.Description,
                Status = overtime.Status,
                RecordData = overtime.Employee.DisplayName,
            };
        }
    }
}
