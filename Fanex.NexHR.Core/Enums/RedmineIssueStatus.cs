namespace Fanex.NexHR.Core.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum RedmineIssueStatus
    {
        [Display(Name = "New")]
        New = 1,

        [Display(Name = "In Progress")]
        InProgress = 2,

        [Display(Name = "Resolved")]
        Resolved = 3,

        [Display(Name = "Closed")]
        Closed = 5,

        [Display(Name = "Rejected")]
        Rejected = 6,

        [Display(Name = "Pending")]
        Pending = 9,
    }
}