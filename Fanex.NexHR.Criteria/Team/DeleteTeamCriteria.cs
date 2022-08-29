namespace Fanex.NexHR.Criteria.Team
{
    public class DeleteTeamCriteria : BaseCommand
    {
        public int Id { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return Id > 0;
        }

        public override string GetSettingKey()
        {
            return "EmployeeGroup_Delete";
        }
    }
}