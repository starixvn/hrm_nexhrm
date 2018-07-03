namespace Fanex.NexHR.Core.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum FingerprintScannerStatus
    {
        [Display(Name = "Pending")]
        Pending = 0,

        [Display(Name = "Approve")]
        Approve = 1,
    }
}