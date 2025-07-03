namespace Fanex.NexHR.Models
{
    public class EmployeesMissedLogTime
    {
        public string TeamCode { get; set; }
        public int GroupTeamId { get; set; }
        public int EmpID { get; set; }
        public string Code { get; set; }
        public string NickName { get; set; }
        public string JobTitle { get; set; }
        public bool IsLeader { get; set; }
        public int MissedCount { get; set; }
        public string ConversationId { get; set; }
        public string DomainUsername { get; set; }
    }
}
