namespace Fanex.NexHR.ViewModels.Promotion
{
    using Fanex.NexHR.Resources;
    using System.ComponentModel.DataAnnotations;

    public class EditPromotionViewModel : CreatePromotionViewModel
    {
        [Required]
        [Display(Name = "Id", ResourceType = typeof(Labels))]
        public int? Id { get; set; }
    }
}
