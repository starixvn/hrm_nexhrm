using System;

namespace Fanex.NexHR.Models.RedmineAPI
{
    public class RedmineIssue
    {
        public int IssueId { get; set; }

        public string Subject { get; set; }

        public string Description { get; set; }

        public string ProjectName { get; set; }

        public string Tracker { get; set; }

        public string Priority { get; set; }

        public string Status { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? DueDate { get; set; } = null;

        public string CreatedBy { get; set; }

        public string Assignee { get; set; }

        public bool IsOverDue
        {
            get
            {
                if (DueDate.HasValue)
                {
                    return DueDate < DateTimeOffset.Now.Date;
                }

                return false;
            }
        }
    }
}