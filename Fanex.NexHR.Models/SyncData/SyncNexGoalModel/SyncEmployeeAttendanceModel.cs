using System;

namespace Fanex.NexHR.Models.SyncData.SyncNexGoalModel;

public class SyncEmployeeAttendanceModel
{
    public int EmpId { get; set; }

    public DateTime AttendanceDate { get; set; }

    public DateTime InFixed { get; set; }

    public DateTime OutFixed { get; set; }

    public bool LateIn { get; set; }

    public bool EarlyOut { get; set; }

    public decimal? LeaveDay { get; set; }

    public int LeaveTypeId { get; set; }

    public int LeaveTypeId2 { get; set; }

    public int Absence { get; set; }

    public decimal? TotalWorkHours { get; set; }
}