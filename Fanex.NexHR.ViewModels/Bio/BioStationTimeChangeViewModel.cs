namespace Fanex.NexHR.ViewModels.Bio
{
    using System;
    using Common;

    public class BioStationTimeChangeViewModel : PagerOptionModel
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
