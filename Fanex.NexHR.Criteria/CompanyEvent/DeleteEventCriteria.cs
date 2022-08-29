namespace Fanex.NexHR.Criteria.CompanyEvent
{
    public class DeleteEventCriteria : BaseCommand
    {
        public int Id { get; set; }

        public override bool IsValid()
        {
            return Id > 0;
        }

        public override string GetSettingKey()
        {
            return "CompanyEvent_Delete";
        }
    }
}