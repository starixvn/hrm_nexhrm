namespace Fanex.NexHR.Criteria.Request
{
    public class DeleteRequestCriteria : BaseCommand
    {
        public int Id { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_Delete";
        }

        public override bool IsValid()
        {
            return Id > 0;
        }
    }
}