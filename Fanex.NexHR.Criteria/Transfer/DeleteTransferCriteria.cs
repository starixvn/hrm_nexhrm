namespace Fanex.NexHR.Criteria.Transfer
{
    public class DeleteTransferCriteria : BaseCommand
    {
        public int Id { get; set; }

        public override string GetSettingKey() => "HistoryWork_Delete";

        public override bool IsValid() => Id > 0;
    }
}
