namespace Fanex.NexHR.Models.Bio
{
    using System;
    using Fanex.NexHR.Core.Enums;

    public class Card
    {
        public Employee Employee { get; set; }

        public int BioUserId { get; set; }

        public int CardId { get; set; }

        public int Type { get; set; }

        public string CardNo { get; set; }

        public string CustomNo { get; set; }

        public DateTime RegisterDate { get; set; }

        public FingerprintScannerAction Status { get; set; }
    }
}