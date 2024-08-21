namespace Fanex.NexHR.Models.Notification
{
    public class Notification
    {
        private const int years = 2;
        private const int months = 3;
        private const int timeMissMonths = 2;

        public int EmpId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string DisplayName => NickName + (string.IsNullOrWhiteSpace(FullName) ? string.Empty : " (" + FullName + ")");

        public int LateInEarlyOutInYear { get; set; }

        public int LateInEarlyOutInMonth { get; set; }

        public int TimeMisssingCheckInOutThisMonth { get; set; }

        public bool ToeicExpired { get; set; }

        public string QuantityOfNewAlert
        {
            get
            {
                int quantity = 0;
                if (LateInEarlyOutInYear >= years)
                {
                    quantity += 1;
                }

                if (LateInEarlyOutInMonth >= months)
                {
                    quantity += 1;
                }

                if (TimeMisssingCheckInOutThisMonth >= timeMissMonths)
                {
                    quantity += 1;
                }

                if (ToeicExpired)
                {
                    quantity += 1;
                }

                return quantity.ToString() == "0" ? string.Empty : quantity.ToString();
            }
        }
    }
}