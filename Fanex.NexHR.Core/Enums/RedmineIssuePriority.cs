namespace Fanex.NexHR.Core.Enums
{
    using Attributes;

    public enum RedmineIssuePriority
    {
        [StringValue("Low")]
        Low = 1,

        [StringValue("Normal")]
        Normal = 2,

        [StringValue("High")]
        High = 3,

        [StringValue("Urgent")]
        Urgent = 4,

        [StringValue("Immediate")]
        Immediate = 5,
    }
}