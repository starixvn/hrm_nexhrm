namespace Fanex.NexHR.Criteria.EmployeeExperience
{
    using Fanex.Data.Repository;

    public class GetEmployeeExperienceByIdCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeExperience_GetById";
        }

        public override bool IsValid() => Id > 0;
    }
}
