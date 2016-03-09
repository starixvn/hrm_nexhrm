namespace Fanex.NexHR.ViewModels.Report
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.ViewModels.Common;

    public class EmployeeAttendanceViewModel : PagerOptionModel
    {
        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }

        [Display(Name = "Employee")]
        public int? EmployeeId { get; set; }

        public string Employee { get; set; }

        public int? GroupId { get; set; }

        public short AttendanceType { get; set; }

        public int DateOption { get; set; }

        public bool ExcludeUserDelete { get; set; }

        public IList<Team> Teams { get; set; }
    }
}