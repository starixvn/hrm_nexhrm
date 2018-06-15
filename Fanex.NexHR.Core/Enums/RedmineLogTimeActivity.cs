namespace Fanex.NexHR.Core.Enums
{
    using System.ComponentModel.DataAnnotations;
    using Attributes;

    public enum RedmineLogTimeActivity
    {
        [StringValue("Analyzing")]
        [Display(Name = "Analyzing")]
        Analyzing = 21,

        [StringValue("Development")]
        [Display(Name = "Development")]
        Development = 9,

        [StringValue("Testing")]
        [Display(Name = "Testing")]
        Testing = 11,

        [StringValue("Meeting")]
        [Display(Name = "Meeting")]
        Meeting = 16,

        [StringValue("Support")]
        [Display(Name = "Support")]
        Support = 17,

        [StringValue("Researching")]
        [Display(Name = "Researching")]
        Researching = 19,

        [StringValue("Others")]
        [Display(Name = "Others")]
        Others = 42,

        [StringValue("Overtime - Support")]
        [Display(Name = "Overtime - Support")]
        OvertimeSupport = 27,

        [StringValue("Overtime - Development")]
        [Display(Name = "Overtime - Development")]
        OvertimeDevelopment = 25,

        [StringValue("Overtime - Testing")]
        [Display(Name = "Overtime - Testing")]
        OvertimeTesting = 26,
    }
}