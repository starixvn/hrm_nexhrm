namespace Fanex.NexHR.Models
{
    public class TotalWorkingHours
    {
        public Employee Employee { get; set; }

        public int WorkingDay { get; set; }

        public float ActualWorkingDay { get; set; }

        public float AnnualLeave { get; set; }

        public float PaidLeave { get; set; }

        public float SickLeave { get; set; }

        public float InsuranceLeave { get; set; }

        public float UnpaidLeave { get; set; }

        public int LateIn { get; set; }

        public int EarlyOut { get; set; }

        public float TotalHours { get; set; }

        public float AverageHours { get; set; }

        public string Code => Employee != null ? Employee.Code : string.Empty;

        public string FullName => Employee != null ? Employee.FullName : string.Empty;

        public string NickName => Employee != null ? Employee.NickName : string.Empty;

        public string Team => Employee != null ? Employee.CodeOfTeam : string.Empty;

    }
}
