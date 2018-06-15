namespace Fanex.NexHR.Core.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum RedmineLogTimeActivity
    {
        [Display(Name = "Analyzing")]
        Analyzing = 21,

        [Display(Name = "Development")]
        Development = 9,

        [Display(Name = "Testing")]
        Testing = 11,

        [Display(Name = "Meeting")]
        Meeting = 16,

        [Display(Name = "Support")]
        Support = 17,

        [Display(Name = "Researching")]
        Researching = 19,

        [Display(Name = "Others")]
        Others = 42,

        [Display(Name = "Overtime - Support")]
        OvertimeSupport = 27,

        [Display(Name = "Overtime - Development")]
        OvertimeDevelopment = 25,

        [Display(Name = "Overtime - Testing")]
        OvertimeTesting = 26,
    }
}