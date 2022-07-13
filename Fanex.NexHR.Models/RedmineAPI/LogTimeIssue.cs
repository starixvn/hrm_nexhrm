namespace Fanex.NexHR.Models.RedmineAPI
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Core.Enums;
    using Resources;

    public class LogTimeIssue
    {
        private const int length = 255;
        private const int maxRange = 24;

        [Required]
        public int IssueId { get; set; }

        public string IssueName { get; set; }

        [Required]
        [Display(Name = nameof(Labels.SpentOn), ResourceType = typeof(Labels))]
        public DateTime SpentOn { get; set; }

        [Required]
        [Range(0, maxRange, ErrorMessageResourceType = typeof(Labels), ErrorMessageResourceName = nameof(Labels.HoursLogtimeErrorMessage))]
        public double? Hours { get; set; }

        [StringLength(length)]
        public string Comments { get; set; }

        [Required]
        public RedmineLogTimeActivity Activity { get; set; }
    }
}