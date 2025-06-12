using System;

namespace Fanex.NexHR.Models.SyncData.SyncNexGoalModel;

public class GroupModel
{
    public int GroupId { get; set; }

    public string Name { get; set; }

    public string Code { get; set; }

    public int DepartmentId { get; set; }

    public int ParentId { get; set; }

    public int Rank { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime ModifiedDate { get; set; }
}