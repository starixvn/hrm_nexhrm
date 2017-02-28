namespace Fanex.NexHR.Core.Enums
{
    using System.ComponentModel;

    public enum RequestMonitoringType
    {
        [Description("All")]
        All = 0,

        [Description("Leave")]
        Leave = 1,

        [Description("Check In/Out")]
        CheckInOut = 2,

        [Description("Late In/Early Out")]
        LateInOut = 3,

        [Description("Absent")]
        Absent = 4
    }
}