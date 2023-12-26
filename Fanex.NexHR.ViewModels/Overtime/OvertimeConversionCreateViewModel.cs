
namespace Fanex.NexHR.ViewModels.Overtime
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class OvertimeConversionCreateViewModel
    {
        public OvertimeConversionCreateViewModel()
        {
            this.DeductDate = DateTimeOffset.Now.DateTime;
        }

        [Required]
        public int EmpId { get; set; }

        public string Description { get; set; }

        [Required]
        [Range(0, float.MaxValue, ErrorMessage = "Deduct can not less than 0")]
        public float Deduct { get; set; }

        [Required]
        [Display(Name = "Deduct Date")]
        public DateTime DeductDate { get; set; }
    }
}
