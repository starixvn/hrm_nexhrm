namespace Fanex.NexHR.Criteria.CompanyEvent
{
    public class DeleteEventCommand : BaseCommand
    {
        public int Id { get; set; }

        public override bool IsValid() => Id > 0;

        public override string GetSettingKey() => "CompanyEvent_Delete";
    }
}