namespace Fanex.NexHR.Criteria.EmployeeExperience
{
    public class DeleteEmployeeExperienceCriteria : BaseCommand
    {
        public int Id { get; set; }
        public override string GetSettingKey()
        {
            return "EmployeeExperience_Delete";
        }

        public override bool IsValid() => Id > 0;
    }
}
