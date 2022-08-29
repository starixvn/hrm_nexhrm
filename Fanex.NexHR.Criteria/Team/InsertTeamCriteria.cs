namespace Fanex.NexHR.Criteria.Team
{
    public class InsertTeamCriteria : BaseCommand
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public int LeaderId { get; set; }

        public int AssistantLeaderId { get; set; }

        public int? DepartmentId { get; set; }

        public int? ParentId { get; set; }

        public int CreatedBy { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Code) && !string.IsNullOrWhiteSpace(Name);
        }

        public override string GetSettingKey()
        {
            return "EmployeeGroup_Insert";
        }
    }
}