namespace Fanex.NexHR.Criteria.HistoryWork
{
    using Fanex.Data.Repository;

    public class DeleteHistoryWorkCriteria : CriteriaBase
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
