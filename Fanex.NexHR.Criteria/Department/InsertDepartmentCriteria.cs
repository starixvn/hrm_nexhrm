namespace Fanex.NexHR.Criteria.Department
{
    public class InsertDepartmentCriteria : BaseCommand
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public int? ParentId { get; set; }

        public int? ManagerId { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Department_Insert";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Code) && !string.IsNullOrWhiteSpace(Name);
        }
    }
}