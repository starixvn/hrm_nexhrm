namespace Fanex.NexHR.Models.Report;

using System;

public class LateInEarlyOutDetail
{
    public Employee Employee { get; set; }

    public DateTime WorkDate { get; set; }

    public int TypeId { get; set; }

    public int LeaveDeductionId { get; set; }

    public int Minutes { get; set; }

    public DateTime ModifiedDate { get; set; }
}