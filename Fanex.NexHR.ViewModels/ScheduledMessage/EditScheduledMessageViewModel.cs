namespace Fanex.NexHR.ViewModels.ScheduledMessage
{
    using Fanex.NexHR.Resources;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class EditScheduledMessageViewModel
    {
        public int ScheduledMessageId { get; set; }

        [Required]
        [Display(ResourceType = typeof(Labels), Name = nameof(Labels.Group))]
        public string ConversationId { get; set; }

        [Required]
        [Display(ResourceType = typeof(Labels), Name = nameof(Labels.SendDate))]
        public DateTime SendDate { get; set; }

        [Required]
        public int Hour { get; set; }

        [Required]
        public int Minute { get; set; }

        [Required]
        public string Content { get; set; }

        public SelectList GroupConversations { get; set; }
    }
}