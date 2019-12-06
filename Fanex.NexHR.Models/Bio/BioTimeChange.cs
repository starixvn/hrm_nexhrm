namespace Fanex.NexHR.Models.Bio
{
    using System;

    public class BioTimeChange
    {
        public int DeviceId { get; set; }

        public string DeviceName { get; set; }

        public string DisplayName { get; set; }

        public DateTime TimeChange { get; set; }

        public DateTime PreviousLogTime { get; set; }

        public DateTime NextLogTime { get; set; }

        public bool Warning
        {
            get
            {
                const byte MaxMinuteChange = 2;

                if (PreviousLogTime <= TimeChange && TimeChange <= NextLogTime)
                {
                    return false;
                }

                if (Math.Abs((TimeChange - PreviousLogTime).Minutes) <= MaxMinuteChange || Math.Abs((TimeChange - NextLogTime).Minutes) <= MaxMinuteChange)
                {
                    return false;
                }

                return true;
            }
        }
    }
}