namespace Fanex.NexHR.ViewModels.Calendar
{
    using System.Collections.Generic;
    using Models;
    using Models.Calendar;

    public class WorkingCalendarViewModel
    {
        public int EmployeeId { get; set; }

        public string Employee { get; set; }

        public int? TeamId { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        public IList<Team> Teams { get; set; }

        public IList<WorkingSchedule> WorkingSchedules { get; set; }
    }
}