namespace Fanex.NexHR.Core.Enums
{
    using Attributes;

    public enum RequestType
    {
        [StringValue("Check In")]
        CheckIn = 1,

        [StringValue("Check Out")]
        CheckOut = 2,

        [StringValue("Late In")]
        LateIn = 3,

        [StringValue("Early Out")]
        EarlyOut = 4,

        [StringValue("Change Shift")]
        ChangeShift = 5,

        [StringValue("Leave")]
        Leave = 6
    }
}