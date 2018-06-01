namespace Fanex.NexHR.Models.RedmineAPI
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class LogTimeIssue
    {
        [Required]
        public int IssueId { get; set; }

        public string IssueName { get; set; }

        [Required]
        [Display(Name = "Spent On")]
        public DateTime SpentOn { get; set; }

        [Required]
        [Range(0.01, 24, ErrorMessage = "{0} must be between {1} and {2}")]
        public double Hours { get; set; }

        [StringLength(255)]
        public string Comments { get; set; }
    }
}