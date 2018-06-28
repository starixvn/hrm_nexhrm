namespace Fanex.NexHR.Core.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum FingerprintScannerType
    {
        [Display(Name = "Fingerprint")]
        Fingerprint = 1,

        [Display(Name = "Card")]
        Card = 2,
    }
}