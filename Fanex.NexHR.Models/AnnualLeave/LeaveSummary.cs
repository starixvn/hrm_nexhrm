namespace Fanex.NexHR.Models.AnnualLeave
{
    public class LeaveSummary : TotalOfEachMonth
    {
        public int EmpId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        public int TeamId { get; set; }

        public string TeamCode { get; set; }

        public string TeamName { get; set; }

        public string PayrollCompanyName { get; set; }

        public float AnnualLeaveLastYear { get; set; }

        public float AnnualLeaveCurrentYear { get; set; }

        public float LeaveBalance { get; set; }
    }
}