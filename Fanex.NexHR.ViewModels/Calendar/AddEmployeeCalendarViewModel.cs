namespace Fanex.NexHR.ViewModels
{
    using System;

    public class AddEmployeeCalendarViewModel
    {
        public int? AddEmployeeId { get; set; }

        public string AddEmployee { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public bool SetOffDay { get; set; }
    }
}