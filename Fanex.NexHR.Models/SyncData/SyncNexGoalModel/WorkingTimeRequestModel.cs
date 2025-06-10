using System;

namespace Fanex.NexHR.Models.SyncData.SyncNexGoalModel;

public class WorkingTimeRequestModel
{
    public int EmpId { get; set; }

    public int RequestType { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public decimal? TotalDays { get; set; }

    public int Status { get; set; }
}