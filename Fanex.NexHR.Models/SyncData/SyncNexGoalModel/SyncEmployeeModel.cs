using System;

namespace Fanex.NexHR.Models.SyncData.SyncNexGoalModel;

public class SyncEmployeeModel
{
    public int EmployeeId { get; set; }
    public string DomainUsername { get; set; }
    public string Code { get; set; }
    public string FullName { get; set; }
    public string NickName { get; set; }
    public int GroupId { get; set; }
    public bool Enabled { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime JoinedDate { get; set; }
    public DateTime ConfirmedDate { get; set; }
    public int JobTitleId { get; set; }
    public string Email { get; set; }
    public string ConversationId { get; set; }
    public DateTime ModifiedDate { get; set; }
}