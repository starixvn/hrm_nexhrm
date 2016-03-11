namespace Fanex.NexHR.Core.Enums
{
    using Fanex.NexHR.Core.Attributes;

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
