namespace Fanex.NexHR.Criteria.HistoryWork
{
    public class DeleteHistoryWorkCriteria : BaseCommand
    {
        public int Id { get; set; }

        public override bool IsValid()
        {
            return Id > 0;
        }

        public override string GetSettingKey()
        {
            return "HistoryWork_Delete";
        }
    }
}