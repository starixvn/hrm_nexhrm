namespace Fanex.NexHR.Models.RedmineAPI
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Resources;

    public class LogTimeIssue
    {
        [Required]
        public int IssueId { get; set; }

        public string IssueName { get; set; }

        [Required]
        [Display(Name = nameof(Labels.SpentOn), ResourceType = typeof(Labels))]
        public DateTime SpentOn { get; set; }

        [Required]
        [Range(0, 24, ErrorMessageResourceType = typeof(Labels), ErrorMessageResourceName = nameof(Labels.HoursLogtimeErrorMessage))]
        public double? Hours { get; set; }

        [StringLength(255)]
        public string Comments { get; set; }
    }
}