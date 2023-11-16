namespace Fanex.NexHR.Core.Enums
{
    using Attributes;

    public enum CVSource
    {
        [StringValue("ITviec")]
        ITviec = 1,

        [StringValue("VNWorks")]
        VNWorks = 2,

        [StringValue("TopCV")]
        TopCV = 3,

        [StringValue("TopDev")]
        TopDev = 4,

        [StringValue("FreeC")]
        FreeC = 5,

        [StringValue("LinkedIn")]
        LinkedIn = 6,

        [StringValue("Facebook")]
        Facebook = 7,

        [StringValue("Messenger")]
        Messenger = 8,

        [StringValue("Email")]
        Email = 9,

        [StringValue("Referral")]
        Referral = 10,

        [StringValue("Connection")]
        Connection = 11,

        [StringValue("Hunting")]
        Hunting = 12
    }
}