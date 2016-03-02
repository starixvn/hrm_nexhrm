namespace Fanex.NexHR.ViewModels.TotalWorkingHours
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Models;

    public class TotalWorkingHoursViewModel
    {
        [DataType(DataType.DateTime)]
        public DateTime FromDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime ToDate { get; set; }

        public string Team { get; set; }

        public int? TeamId { get; set; }

        public string Employee { get; set; }

        public int EmployeeId { get; set; }

        public int DateOption { get; set; }

        public bool IsExportExcel { get; set; }

        public IList<Team> Teams { get; set; }
    }
}