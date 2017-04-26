namespace Fanex.NexHR.Core.Enums
{
    using Attributes;

    public enum LeaveConversionType
    {
        [StringValue("All")]
        All = 0,

        [StringValue("LateInEarlyOutDeduction")]
        LateInEarlyOutDeduction = 1,

        [StringValue("OvertimeConversion")]
        OvertimeConversion = 2
    }
}
