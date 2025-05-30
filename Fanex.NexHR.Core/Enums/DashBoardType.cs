namespace Fanex.NexHR.Core.Enums;

using Attributes;

public enum DashBoardType
{
    [StringValue("None")]
    None = 0,

    [StringValue("Leave")]
    Leave = 1,

    [StringValue("CheckInOut")]
    CheckInOut = 2,

    [StringValue("MissingCheckInOut")]
    MissingCheckInOut = 3,

    [StringValue("LateInEarlyOut")]
    LateInEarlyOut = 4,

    [StringValue("Absent")]
    Absent = 5
}