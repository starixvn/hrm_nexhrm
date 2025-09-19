namespace Fanex.NexHR.ViewModels.Report
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class WorkingHoursStatisticViewModel : WorkingHours
    {
        public int Month { get; set; }

        public int Year { get; set; }

        public DateTime FromDateLastMonth { get; set; }

        public DateTime ToDateLastMonth { get; set; }
    }

    public class WorkingHours
    {
        public int DateOption { get; set; }

        public string Employee { get; set; }

        public int EmployeeId { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime FromDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime ToDate { get; set; }

        public bool IsExportExcel { get; set; }

        public string Team { get; set; }

        public int? TeamId { get; set; }

        public int? DepartmentId { get; set; }

        public int? PayrollCompanyId { get; set; }

        public IList<Team> Teams { get; set; }

        public IEnumerable<Department> Departments { get; set; }

        public List<CompanyModel> Companies { get; set; }

        public string Sort { get; set; }
    }
}