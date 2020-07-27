namespace Fanex.NexHR.Models.Overtime
{
    public class OvertimeSummary : TotalOfEachMonth
    {
        public float BalanceLastYear { get; set; }

        public float BalanceCurrentYear { get; set; }

        public float LeaveConversion { get; set; }

        public float TotalHour { get; set; }

        public float LateInEarlyOut { get; set; }

        public Employee Employee { get; set; }

        public float OvertimeDeduct { get; set; }

        public static string DisplayValue(float value)
        {
            return value.Equals(0) ? "-" : value.ToString("0.0");
        }
    }
}
