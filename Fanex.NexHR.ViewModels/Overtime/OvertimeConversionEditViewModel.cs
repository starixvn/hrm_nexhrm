namespace Fanex.NexHR.ViewModels.Overtime
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Core;

    public class OvertimeConversionEditViewModel
    {
        public OvertimeConversionEditViewModel()
        {
            this.DeductDate = DateTimeOffset.Now.DateTime.ToClientTimeZone();
        }

        [Required]
        public int Id { get; set; }

        public string Description { get; set; }

        [Required]
        [Range(0, float.MaxValue, ErrorMessage = "Deduct can not less than 0")]
        public float Deduct { get; set; }

        [Required]
        [Display(Name = "Deduct Date")]
        public DateTime DeductDate { get; set; }

    }
}
