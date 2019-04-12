namespace Fanex.NexHR.ViewModels.ScheduledMessage
{
    using Fanex.NexHR.Resources;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class EditConversationViewModel
    {
        [Required]
        [Display(ResourceType = typeof(Labels), Name = nameof(Labels.IdNo))]
        public string ConversationId { get; set; }

        [Required]
        [Display(ResourceType = typeof(Labels), Name = nameof(Labels.Name))]
        public string ConversationName { get; set; }

        [Display(ResourceType = typeof(Labels), Name = nameof(Labels.PermittedEmployees))]
        public int[] PermittedEmployeeIds { get; set; }

        public SelectList Employees { get; set; }

        public bool HasHRRole { get; set; }

        public int Id { get; set; }
    }
}