namespace Fanex.NexHR.ViewModels.TrainingAndDevelopment
{
    using Fanex.NexHR.Resources;
    using System.ComponentModel.DataAnnotations;

    public class TrainingAndDevelopmentUpdateViewModel : TrainingAndDevelopmentCreateViewModel
    {
        [Required]
        [Display(Name = "Id", ResourceType = typeof(Labels))]
        public int? Id { get; set; }
    }
}
