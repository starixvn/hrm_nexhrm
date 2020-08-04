namespace Fanex.NexHR.Core.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum OvertimeVerifyType
    {
        [Display(Name = "All")]
        All = 2,

        [Display(Name = "Yes")]
        Yes = 1,

        [Display(Name = "No")]
        No = 0
    }
}
