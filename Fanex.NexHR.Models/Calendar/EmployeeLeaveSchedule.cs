namespace Fanex.NexHR.Models.Calendar
{
    using System.Collections.Generic;

    public class EmployeeLeaveSchedule
    {
        public Employee Employee { get; set; }

        public IList<LeaveSchedule> LeaveSchedules { get; set; }
    }
}