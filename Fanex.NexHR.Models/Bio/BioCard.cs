namespace Fanex.NexHR.Models.Bio
{
    using Core.Enums;

    public class BioCard
    {
        public int CardIn { get; set; }

        public int Type { get; set; }

        public string CardNo { get; set; }

        public string CustomNo { get; set; }

        public FingerprintScannerStatus Status { get; set; }
    }
}