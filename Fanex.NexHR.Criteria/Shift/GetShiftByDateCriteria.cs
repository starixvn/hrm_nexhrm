namespace Fanex.NexHR.Criteria.Shift;

using System;
using Data.Repository;

public class GetShiftByDateCriteria : CriteriaBase
{
    public int EmployeeId { get; set; }

    public DateTime Date { get; set; }

    public override bool IsValid()
    {
        return true;
    }

    public override string GetSettingKey()
    {
        return "EmployeeCalendar_GetShiftByDate";
    }
}