using Fanex.NexHR.Core.Enums;

namespace Fanex.NexHR.Models.Bio
{
    public class Fingerprint
    {
        public Employee Employee { get; set; }

        public int BioUserId { get; set; }

        public int Index { get; set; }

        public FingerprintScannerStatus Status { get; set; }
    }
}