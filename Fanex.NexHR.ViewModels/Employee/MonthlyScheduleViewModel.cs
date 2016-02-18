namespace Fanex.NexHR.ViewModels.Employee
{
    using Fanex.NexHR.Models;
    using System.Collections.Generic;

    public class MonthlyScheduleViewModel
    {
        public int EmployeeId { get; set; }

        public string EmployeeNickName { get; set; }

        public int ShiftId { get; set; }

        public IList<Shift> Shifts { get; set; }
    }
}
