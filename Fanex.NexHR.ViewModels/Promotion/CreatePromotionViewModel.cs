namespace Fanex.NexHR.ViewModels.Promotion
{
    using Fanex.NexHR.Resources;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class CreatePromotionViewModel
    {
        [Required]
        [Display(Name = "Employee", ResourceType = typeof(Labels))]
        public int? EmployeeId { get; set; }

        public string Employee { get; set; }

        public string CurrentGrade { get; set; }

        public string PromotedGrade { get; set; }

        public DateTime EffectiveDate { get; set; }
    }
}
