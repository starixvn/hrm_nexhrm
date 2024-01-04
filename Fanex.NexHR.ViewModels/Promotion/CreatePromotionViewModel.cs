namespace Fanex.NexHR.ViewModels.Promotion
{
    using System;
    using Fanex.NexHR.Resources;
    using System.ComponentModel.DataAnnotations;

    public class CreatePromotionViewModel
    {
        [Required]
        [Display(Name = "Employee", ResourceType = typeof(Labels))]
        public int EmployeeId { get; set; }

        public string Employee { get; set; }

        public int OldJobGradeId { get; set; }

        public int JobGradeId { get; set; }

        public DateTime EffectiveDate { get; set; }
    }
}
