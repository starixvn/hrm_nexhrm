namespace Fanex.NexHR.Criteria.Team
{
    public class UpdateTeamCriteria : BaseCommand
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int? DepartmentId { get; set; }

        public int? ParentId { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return Id > 0 && !string.IsNullOrWhiteSpace(Code) && !string.IsNullOrWhiteSpace(Name);
        }

        public override string GetSettingKey()
        {
            return "EmployeeGroup_Update";
        }
    }
}