using System;

namespace Fanex.NexHR.Models.SyncData.SyncNexGoalModel;

public class EmployeeOverTimeModel
{
    public int OvertimeId { get; set; }

    public int EmployeeId { get; set; }

    public decimal? Hour { get; set; }

    public DateTime Date { get; set; }
}