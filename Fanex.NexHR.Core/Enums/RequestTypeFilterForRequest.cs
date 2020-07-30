namespace Fanex.NexHR.Core.Enums
{
    using System.ComponentModel;

    public enum RequestTypeFilterForRequest
    {
        [Description("Leave")]
        Leave = 1,

        [Description("Check In/Out")]
        CheckInOut = 4,

        [Description("All")]
        All = 5
    }
}