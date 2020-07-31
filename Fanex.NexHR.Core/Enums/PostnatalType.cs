namespace Fanex.NexHR.Core.Enums
{
    using Fanex.Common.Extensions;

    public enum PostnatalType
    {
        [StringValue("Late In/Early Out")]
        LateInEarlyOut = 1,

        [StringValue("BreakTime")]
        BreakTime = 2,

        [StringValue("Mix Late In/Early Out And BreakTime")]
        MixLateInEarlyOutAndBreakTime = 3
    }
}
