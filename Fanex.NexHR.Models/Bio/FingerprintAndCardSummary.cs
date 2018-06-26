namespace Fanex.NexHR.Models.Bio
{
    public class FingerprintAndCardSummary
    {
        public Employee Employee { get; set; }

        public int BioUserId { get; set; }

        public int Fingerprint { get; set; }

        public int Card { get; set; }

        public bool IsWarning { get; set; }
    }
}