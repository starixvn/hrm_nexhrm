namespace Fanex.NexHR.Criteria.Overtime
{
    public class RejectRequestCriteria : BaseCommand
    {
        public int Id { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "Overtime_RejectRequest";
        }

        public override bool IsValid()
        {
            return Id > 0;
        }
    }
}