namespace Fanex.NexHR.Core.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum FingerprintScannerType
    {
        [Display(Name = "Fingerprint")]
        Fingerprint = 0,

        [Display(Name = "Card")]
        Card = 1,
    }
}