namespace Fanex.NexHR.ViewModels.Overtime
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Models.Overtime;
    using Resources;

    public class OvertimeEditViewModel
    {
        private const double minRange = 0.5;
        private const int maxRange = 8;
        private const int length = 4000;

        public int Id { get; set; }
        [Display(Name = "Date", ResourceType = typeof(Labels))]
        [Required]
        public DateTime Date { get; set; }

        [Display(Name = "Hours", ResourceType = typeof(Labels))]
        [Range(minimum: minRange, maximum: maxRange)]
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
        [StringLength(length)]
        public string Description { get; set; }

        public int Status { get; set; }

        public static OvertimeEditViewModel Parse(Overtime overtime)
        {
            OvertimeEditViewModel editModel = null;
            if (overtime != null)
            {
                editModel = new OvertimeEditViewModel
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

            return editModel;
        }
    }
}
