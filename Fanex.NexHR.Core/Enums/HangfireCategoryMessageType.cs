namespace Fanex.NexHR.Core.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum HangfireCategoryMessageType
    {
        [Display(Name = "New Finger Machine")]
        BioDevice = 1,

        [Display(Name = "BIO Change Time")]
        BioTimeChange = 2,

        [Display(Name = "Fingerprint & Card")]
        BioFingerprintAndCard = 3,

        [Display(Name = "Missing Check In/Check Out")]
        MissingCheckInOut = 4
    }
}