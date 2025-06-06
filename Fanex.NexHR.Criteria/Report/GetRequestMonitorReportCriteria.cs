namespace Fanex.NexHR.Criteria.Report;

using System;
using Data.Repository;

public class GetRequestMonitorReportCriteria : CriteriaBase
{
    public int Role { get; set; }

    public int EmployeeId { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public int RequestType { get; set; }

    public int MaxRequest { get; set; }

    public override string GetSettingKey()
    {
        return "Report_RequestMonitor";
    }

    public override bool IsValid()
    {
        return FromDate <= ToDate;
    }
}