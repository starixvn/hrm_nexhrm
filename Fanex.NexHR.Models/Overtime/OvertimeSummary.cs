namespace Fanex.NexHR.Models.Overtime
{
    public class OvertimeSummary
    {
        public float BalanceLastYear { get; set; }

        public float BalanceCurrentYear { get; set; }

        public float LeaveConversion { get; set; }

        public float Jan { get; set; }

        public float Feb { get; set; }

        public float Mar { get; set; }

        public float Apr { get; set; }

        public float May { get; set; }

        public float Jun { get; set; }

        public float Jul { get; set; }

        public float Aug { get; set; }

        public float Sep { get; set; }

        public float Oct { get; set; }

        public float Nov { get; set; }

        public float Dec { get; set; }

        public float TotalHour { get; set; }

        public float LateInEarlyOut { get; set; }

        public Employee Employee { get; set; }

        public string DisplayValue(float value)
        {
            return value.Equals(0) ? "-" : value.ToString("0.0");
        }
    }
}
