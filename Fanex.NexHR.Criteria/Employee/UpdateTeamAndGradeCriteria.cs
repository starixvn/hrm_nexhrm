namespace Fanex.NexHR.Criteria
{
    public class UpdateTeamAndGradeCriteria : BaseCommand
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Employee_UpdateTeamAndGrade";
        }
    }
}
