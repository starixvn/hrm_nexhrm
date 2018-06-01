namespace Fanex.NexHR.Models.RedmineAPI
{
    using System;
    using Core.Enums;

    public class RedmineIssue
    {
        public int IssueId { get; set; }

        public string Subject { get; set; }

        public string Description { get; set; }

        public string ProjectName { get; set; }

        public double EstimatedHours { get; set; }

        public double SpentHours { get; set; }

        public string Author { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime DueDate { get; set; }

        public RedmineIssuePriority Priority { get; set; }

        public RedmineIssueStatus Status { get; set; }
    }
}