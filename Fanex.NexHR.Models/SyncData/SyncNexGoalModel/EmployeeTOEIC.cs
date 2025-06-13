using System;

namespace Fanex.NexHR.Models.SyncData.SyncNexGoalModel;

public class EmployeeTOEIC
{
    public int EmpId { get; set; }

    public int? MinimumScore { get; set; }

    public DateTime? InputDate { get; set; }

    public DateTime? LRExamDate { get; set; }

    public int? Listening { get; set; }

    public int? Reading { get; set; }

    public DateTime? ModifiedDate { get; set; }

}