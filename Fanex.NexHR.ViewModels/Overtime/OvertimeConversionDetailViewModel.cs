namespace Fanex.NexHR.ViewModels.Overtime
{
    using Fanex.NexHR.Models;
    using System;
    using System.Collections.Generic;

    public class OvertimeConversionDetailViewModel
    {
        public OvertimeConversionDetailViewModel()
        {
            this.FromDate = new DateTimeOffset(DateTimeOffset.Now.Year, DateTimeOffset.Now.Month, 1, 0, 0, 0, TimeSpan.Zero).DateTime;
            this.ToDate = DateTimeOffset.Now.DateTime;
        }
        public int EmployeeId { get; set; }

        public string Employee { get; set; }

        public int? TeamId { get; set; }

        public int DateOption { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public IList<Team> Teams { get; set; }
    }
}
