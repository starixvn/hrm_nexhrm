namespace Fanex.NexHR.Core.Enums
{
    using Attributes;

    public enum RequestStatuse
    {
        [StringValue("All")]
        All = 0,
        [StringValue("Pending")]
        Pending = 1,

        [StringValue("Approved")]
        Approve = 2,

        [StringValue("Rejected")]
        Reject = 3,

        [StringValue("Verified")]
        Verified = 4,

        [StringValue("Void")]
        Void = 5,
    }
}