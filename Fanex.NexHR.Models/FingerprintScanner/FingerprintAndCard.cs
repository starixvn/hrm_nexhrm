namespace Fanex.NexHR.Models.FingerprintScanner
{
    public class FingerprintAndCard
    {
        public Employee Employee { get; set; }

        public int Fingerprint { get; set; }

        public int Card { get; set; }

        public bool IsWarnning { get; set; }
    }
}