using Fanex.NexHR.Core.Enums;

namespace Fanex.NexHR.Models.Bio
{
    public class Fingerprint
    {
        public Employee Employee { get; set; }

        public int BioUserId { get; set; }

        public int Index { get; set; }

        public byte[] Template { get; set; }

        public int TemplateCS { get; set; }

        public int TemplateIndex { get; set; }

        public int FingerIndex { get; set; }

        public FingerprintScannerAction Status { get; set; }
    }
}