namespace Fanex.NexHR.Core.Enums;

using Attributes;

public enum GeneralRequestType
{
    [StringValue("Leave")]
    Leave = 0,

    [StringValue("LateInEarlyOut")]
    LateInEarlyOut = 1,

    [StringValue("CheckInOut")]
    CheckInOut = 2,
}