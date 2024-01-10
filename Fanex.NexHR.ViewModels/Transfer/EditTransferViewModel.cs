namespace Fanex.NexHR.ViewModels.Transfer
{
    using Fanex.NexHR.Resources;
    using System.ComponentModel.DataAnnotations;

    public class EditTransferViewModel : CreateTransferViewModel
    {
        [Required]
        [Display(Name = "Id", ResourceType = typeof(Labels))]
        public int Id { get; set; }
    }
}
