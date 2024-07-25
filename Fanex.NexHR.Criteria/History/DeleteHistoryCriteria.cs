namespace Fanex.NexHR.Criteria.History
{
    public class DeleteHistoryCriteria : BaseCommand
    {
        public int Id { get; set; }

        public override string GetSettingKey() => "History_Delete";

        public override bool IsValid() => Id > 0;
    }
}
