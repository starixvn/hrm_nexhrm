namespace Fanex.NexHR.Criteria.EmployeeWorkFromHome
{
    public class DeleteEmployeeWorkFromHomeCriteria : BaseCommand
    {
        public int EmployeeWorkFromHomeId { get; set; }
        public int DeletedBy { get; set; }

        public override string GetSettingKey()
        {
            return "EmployeeWorkFromHome_Delete";
        }

        public override bool IsValid()
        {
            return EmployeeWorkFromHomeId > 0 && DeletedBy > 0;
        }
    }
}