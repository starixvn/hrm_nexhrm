namespace Fanex.NexHR.Criteria.Report;

using System;

public class TimeRecordUpdateTypeCriteria : BaseCommand
{
    public int RecordTimeId { get; set; }
    public int EmployeeId { get; set; }

    public short? InOutType { get; set; }

    public int ModifiedBy { get; set; }

    public DateTime ModifiedDate { get; set; }

    public override string GetSettingKey()
    {
        return "TimeRecord_UpdateType";
    }

    public override bool IsValid()
    {
        return true;
    }
}