namespace Fanex.NexHR.Core.Enums
{
    using Attributes;

    public enum CVStatus
    {
        [StringValue("OK")]
        OK = 1,

        [StringValue("KIV")]
        KIV = 2,

        [StringValue("Rejected")]
        Rejected = 3
    }
}