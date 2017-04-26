namespace Fanex.NexHR.Criteria.Department
{
    using Data.Repository;

    public class DeleteDepartmentCriteria : CriteriaBase
    {
        public int DepartmentId { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Department_Delete";
        }

        public override bool IsValid()
        {
            return DepartmentId > 0;
        }
    }
}
