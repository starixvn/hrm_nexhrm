namespace Fanex.NexHR.Criteria.Report
{
    using System;
    using Core.Attributes;
    using Data;
    using Data.Repository;

    public class GetEmployeeAttendanceReportByTypeCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "fromtime")]
        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "totime")]
        public DateTime ToDate { get; set; }

        [SpParam(SqlParamName = "empid")]
        public int? EmployeeId { get; set; }

        public int? GroupId { get; set; }

        public int? DepartmentId { get; set; }

        public bool ExcludeUserDelete { get; set; }

        [SpParam(SqlParamName = "Type")]
        public short ReportType { get; set; }

        [Output]
        public int? TotalItem { get; set; } = 0;

        public int? ItemsPerPage { get; set; }

        public int? PageIndex { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeAttendance_GetByType";
        }

        public override bool IsValid()
        {
            return FromDate <= ToDate;
        }
    }
}