using System.Collections.Generic;

namespace Fanex.NexHR.Models.Calendar
{
    public class EmployeeWorkingSchedule
    {
        public Employee Employee { get; set; }

        public IList<WorkingSchedule> Schedules { get; set; }
    }
}