namespace Fanex.NexHR.Core.Enums
{
    using Attributes;

    public enum CVStatus
    {
        [StringValue("All")]
        ALL = 0,

        [StringValue("OK")]
        OK = 1,

        [StringValue("KIV")]
        KIV = 2,

        [StringValue("Rejected")]
        Rejected = 3
    }
}