namespace Fanex.NexHR.ViewModels
{
    using System;

    public class AddEmployeeCalendarViewModel
    {
        public int? EmployeeId { get; set; }

        public string Employee { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }
}
