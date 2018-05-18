namespace Fanex.NexHR.Models.Calendar
{
    using System;
    using System.Collections.Generic;

    public class LeaveDaySummarySchedule
    {
        public IList<EmployeeLeaveSchedule> EmployeeLeaveShedules { get; set; }

        public int DaysInMonth { get; set; }

        public DateTime Month { get; set; }

        public IList<string> ListDates { get; set; }

        public IList<double> TotalLeaves { get; set; }
    }
}