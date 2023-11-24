namespace Fanex.NexHR.Core.Enums
{
    using Attributes;

    public enum ResultInterview
    {
        [StringValue("KO")]
        KO = 1,

        [StringValue("OK")]
        OK = 2,

        [StringValue("NA")]
        NA = 3,

        [StringValue("OFF")]
        OFF = 4,

        [StringValue("KIV")]
        KIV = 5
    }
}