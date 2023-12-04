namespace Fanex.NexHR.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class CreatePositionViewModel
    {
        [Required]
        [Display(Name = "Code")]
        public string PositionCode { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string PositionName { get; set; }

        public int? DisplayOrder { get; set; }
    }
}