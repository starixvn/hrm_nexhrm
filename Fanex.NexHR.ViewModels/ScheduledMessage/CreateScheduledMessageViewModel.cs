namespace Fanex.NexHR.ViewModels.ScheduledMessage
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.ViewModels.CustomValidationAttribute;

    public class CreateScheduledMessageViewModel
    {
        [Required]
        [Display(ResourceType = typeof(Labels), Name = nameof(Labels.Conversation))]
        public int ConversationId { get; set; }

        [Required]
        [Display(ResourceType = typeof(Labels), Name = nameof(Labels.SendDate))]
        [DataType(DataType.Date)]
        public DateTime SendDate { get; set; }

        [Required]
        public int Hour { get; set; }

        [Required]
        [MultipleOf(Value = 5, ErrorMessageResourceType = typeof(Messages), ErrorMessageResourceName = nameof(Messages.MultipleOf))]
        public int Minute { get; set; }

        [Required]
        public string Content { get; set; }

        public SelectList Conversations { get; set; }
    }
}