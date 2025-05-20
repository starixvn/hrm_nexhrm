using System;

namespace Fanex.NexHR.Models.Export
{
    public class AttendanceExportModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int? EmployeeId { get; set; }
        public int? GroupId { get; set; }
        public int? DepartmentId { get; set; }
        public short AttendanceType { get; set; }
        public bool ExcludeUserDelete { get; set; }
        public string Sort { get; set; }
    }
}