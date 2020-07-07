namespace Fanex.NexHR.Core.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum MessageCategory
    {
        [Display(Name = "New Finger Machine Alert")]
        BioDevice = 1,

        [Display(Name = "BIO Change Time Alert")]
        BioTimeChange = 2,

        [Display(Name = "New Fingerprint & Card Alert")]
        BioFingerprintAndCard = 3,
    }
}