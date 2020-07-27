namespace Fanex.NexHR.ViewModels.Overtime
{
    using Fanex.NexHR.Models;
    using System;
    using System.Collections.Generic;

    public class OvertimeConversionDetailViewModel
    {
        public OvertimeConversionDetailViewModel()
        {
            this.FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            this.ToDate = DateTime.Now;
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
