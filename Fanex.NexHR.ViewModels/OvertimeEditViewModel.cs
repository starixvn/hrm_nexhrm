namespace Fanex.NexHR.ViewModels
{
    using Core.Enums;
    using Models;
    using Resources;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class OvertimeEditViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Date", ResourceType = typeof(Labels))]
        [Required]
        public DateTime Date { get; set; }

        [Display(Name = "Hours", ResourceType = typeof(Labels))]
        [Range(minimum: 0.5, maximum: 8)]
        public float Hour { get; set; }

        [Display(Name = "CreatedBy", ResourceType = typeof(Labels))]
        public string CreatedNickName { get; set; }

        [Display(Name = "ModifiedBy", ResourceType = typeof(Labels))]
        public string ModifiedNickName { get; set; }

        [Range(1, int.MaxValue, ErrorMessageResourceName = "RequiredEmployee", ErrorMessageResourceType = typeof(Messages))]
        public int EmployeeId { get; set; }

        [Required(ErrorMessageResourceName = "RequiredEmployee", ErrorMessageResourceType = typeof(Messages))]
        public string EmployeeDisplayName { get; set; }

        [Display(Name = "Description", ResourceType = typeof(Labels))]
        [Required]
        [StringLength(4000)]
        public string Description { get; set; }

        public int Status { get; set; }

        public static OvertimeEditViewModel Parse(Overtime overtime)
        {
            return new OvertimeEditViewModel()
            {
                Id = overtime.Id,
                Date = overtime.OnDate,
                Hour = overtime.Hour,
                CreatedNickName = overtime.CreatedBy.NickName,
                ModifiedNickName = overtime.ModifiedBy != null ? overtime.ModifiedBy.NickName : string.Empty,
                Description = overtime.Description,
                Status = overtime.Status,
            };
        }
    }
}
