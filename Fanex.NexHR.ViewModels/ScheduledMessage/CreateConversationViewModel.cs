namespace Fanex.NexHR.ViewModels.ScheduledMessage
{
    using Fanex.NexHR.Resources;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.ComponentModel.DataAnnotations;

    public class CreateConversationViewModel
    {
        [Required]
        [Display(ResourceType = typeof(Labels), Name = nameof(Labels.ConversationId))]
        public string SkypeConversationId { get; set; }

        [Required]
        [Display(ResourceType = typeof(Labels), Name = nameof(Labels.Name))]
        public string ConversationName { get; set; }

        [Display(ResourceType = typeof(Labels), Name = nameof(Labels.PermittedEmployees))]
        public int[] PermittedEmployeeIds { get; set; }

        public SelectList Employees { get; set; }

        public bool HasHRRole { get; set; }
    }
}