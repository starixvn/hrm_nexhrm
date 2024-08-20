namespace Fanex.NexHR.ViewModels.ScheduledMessage
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Resources;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class EditScheduledMessageViewModel
    {
        public int ScheduledMessageId { get; set; }

        [Required]
        [Display(ResourceType = typeof(Labels), Name = nameof(Labels.Conversation))]
        public int ConversationId { get; set; }

        [Display(ResourceType = typeof(Labels), Name = nameof(Labels.RepeatEvery))]
        public int? RepeatTypeId { get; set; }

        [Required]
        [Display(ResourceType = typeof(Labels), Name = nameof(Labels.SendDate))]
        [DataType(DataType.Date)]
        public DateTime SendDate { get; set; }

        [Required]
        public int Hour { get; set; }

        [Required]
        public int Minute { get; set; }

        [Required]
        public string Content { get; set; }

        public SelectList Conversations { get; set; }
        public SelectList RepeatTypes { get; set; }
        public int StepNumber { get; set; }

        [Display(ResourceType = typeof(Labels), Name = nameof(Labels.Repeat))]
        public bool IsRepeat { get; set; }
    }
}