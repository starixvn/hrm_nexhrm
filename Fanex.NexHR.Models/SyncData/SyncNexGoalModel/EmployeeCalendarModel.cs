using System;

namespace Fanex.NexHR.Models.SyncData.SyncNexGoalModel;

public class EmployeeCalendarModel
{
    public int EmpId { get; set; }

    public DateTime WorkDate { get; set; }

    public bool IsOffDay { get; set; }

    public bool IsLeaveDay { get; set; }

    public bool IsHoliday { get; set; }

    public decimal? LeaveDay { get; set; }
}