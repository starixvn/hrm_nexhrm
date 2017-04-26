namespace Fanex.NexHR.Criteria.Department
{
    using Data.Repository;

    public class UpdateDepartmentCriteria : CriteriaBase
    {
        public int DepartmentId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int? ParentId { get; set; }

        public int? ManagerId { get; set; }

        public bool? IsDisabled { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Department_Update";
        }

        public override bool IsValid()
        {
            return DepartmentId > 0 && !string.IsNullOrWhiteSpace(Code) && !string.IsNullOrWhiteSpace(Code);
        }
    }
}
