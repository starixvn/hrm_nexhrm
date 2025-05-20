namespace Fanex.NexHR.Models.Overtime
{
    public class OvertimeSummary : TotalOfEachMonth
    {
        public float BalanceLastYear { get; set; }

        public float BalanceCurrentYear { get; set; }

        public float LeaveConversion { get; set; }

        public float TotalHour { get; set; }

        public float LateInEarlyOut { get; set; }

        public int EmployeeId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string DisplayName => NickName + " (" + FullName + ")";

        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        public int TeamId { get; set; }

        public string TeamCode { get; set; }

        public string TeamName { get; set; }

        public float OvertimeDeduct { get; set; }

        public static string DisplayValue(float value)
        {
            return value.Equals(0) ? "-" : value.ToString("0.0");
        }
    }
}
