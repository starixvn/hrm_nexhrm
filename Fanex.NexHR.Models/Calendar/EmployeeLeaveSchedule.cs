namespace Fanex.NexHR.Models.Calendar
{
    using System.Collections.Generic;

    public class EmployeeLeaveSchedule
    {
        public int EmpId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string TeamCode { get; set; }

        public IList<LeaveSchedule> LeaveSchedules { get; set; }
    }
}