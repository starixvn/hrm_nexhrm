namespace Fanex.NexHR.ViewModels
{
    using System.Collections.Generic;
    using Core.Enums;
    using Models;

    public class FingerprintAndCardViewModel
    {
        public int EmployeeId { get; set; }

        public string Employee { get; set; }

        public int? TeamId { get; set; }

        public FingerprintScannerType? Type { get; set; }

        public IList<Team> Teams { get; set; }
    }
}