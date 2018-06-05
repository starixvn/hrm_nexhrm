namespace Fanex.NexHR.Models.Notification
{
    using System;
    using System.Collections.Generic;

    public class Notification
    {
        public Employee Employee { get; set; }

        public int LateInEarlyOutInYear { get; set; }

        public int LateInEarlyOutInMonth { get; set; }

        public int TimeMisssingCheckInOutThisMonth { get; set; }

        public bool ToeicExpired { get; set; }

        public IList<DateTime> ForgetLogtimeDates { get; set; } = new List<DateTime>();

        public string QuantityOfNewAlert
        {
            get
            {
                int quantity = 0;
                if (LateInEarlyOutInYear >= 2)
                {
                    quantity += 1;
                }

                if (LateInEarlyOutInMonth >= 3)
                {
                    quantity += 1;
                }

                if (TimeMisssingCheckInOutThisMonth >= 2)
                {
                    quantity += 1;
                }

                if (ToeicExpired)
                {
                    quantity += 1;
                }

                if (ForgetLogtimeDates.Count > 0)
                {
                    quantity += 1;
                }

                return quantity.ToString() == "0" ? string.Empty : quantity.ToString();
            }
        }
    }
}