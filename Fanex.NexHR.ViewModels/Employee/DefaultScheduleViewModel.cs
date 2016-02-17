namespace Fanex.NexHR.ViewModels.Employee
{
    using Fanex.NexHR.Models;
    using System.Collections.Generic;

    public class DefaultScheduleViewModel
    {
        public int EmployeeId { get; set; }

        public string EmployeeNickName { get; set; }

        public int? MonShiftId { get; set; }

        public int? TueShiftId { get; set; }

        public int? WedShiftId { get; set; }

        public int? ThuShiftId { get; set; }

        public int? FriShiftId { get; set; }

        public int? SatShiftId { get; set; }

        public int? SunShiftId { get; set; }

        public IList<Shift> Shifts { get; set; }
    }
}
