namespace Fanex.NexHR.Criteria.EmployeeExperience
{
    using Fanex.Data.Repository;

    public class GetEmployeeExperienceByEmpIdCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeExperience_GetByEmpId";
        }

        public override bool IsValid() => EmpId > 0;
    }
}
