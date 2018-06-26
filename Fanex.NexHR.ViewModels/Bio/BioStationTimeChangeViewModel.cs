namespace Fanex.NexHR.ViewModels.Bio
{
    using System;
    using System.Collections.Generic;
    using Models.Bio;

    public class BioStationTimeChangeViewModel
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public IList<BioTimeChange> TimeChanges { get; set; }
    }
}
