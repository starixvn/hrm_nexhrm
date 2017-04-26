namespace Fanex.NexHR.Criteria.Department
{
    using Data.Repository;

    public class GetAllDepartmentCriteria : CriteriaBase
    {
        public int? ParentId { get; set; }

        public bool? IsDisabled { get; set; }

        public override string GetSettingKey()
        {
            return "Department_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
