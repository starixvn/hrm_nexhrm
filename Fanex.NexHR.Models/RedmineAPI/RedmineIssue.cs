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
    }
}