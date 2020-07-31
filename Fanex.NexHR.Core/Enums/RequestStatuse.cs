namespace Fanex.NexHR.Core.Enums
{
    using Attributes;

    public enum RequestStatuse
    {
        [StringValue("Pending")]
        Pending = 1,

        [StringValue("Approved")]
        Approve = 2,

        [StringValue("Rejected")]
        Reject = 3
    }
}