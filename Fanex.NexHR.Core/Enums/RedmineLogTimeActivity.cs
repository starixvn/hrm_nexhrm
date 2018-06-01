namespace Fanex.NexHR.Core.Enums
{
    using Attributes;

    public enum RedmineLogTimeActivity
    {
        [StringValue("Analyzing")]
        Analyzing = 21,

        [StringValue("Development")]
        Development = 9,

        [StringValue("Testing")]
        Testing = 11,

        [StringValue("Meeting")]
        Meeting = 16,

        [StringValue("Support")]
        Support = 17,

        [StringValue("Researching")]
        Researching = 19,

        [StringValue("Others")]
        Others = 42,

        [StringValue("Overtime - Support")]
        OvertimeSupport = 27,

        [StringValue("Overtime - Development")]
        OvertimeDevelopment = 25,

        [StringValue("Overtime - Testing")]
        OvertimeTesting = 26,
    }
}