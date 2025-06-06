namespace Fanex.NexHR.Core.Enums;

using Attributes;

public enum Command
{
    [StringValue("Approve")]
    Approve = 1,

    [StringValue("Disapprove")]
    Disapprove = 2,

    [StringValue("Reject")]
    Reject = 3
}