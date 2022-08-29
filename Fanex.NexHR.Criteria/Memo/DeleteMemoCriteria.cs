namespace Fanex.NexHR.Criteria.Memo
{
    /// <summary>
    /// Delete Memo Criteria.
    /// </summary>
    public class DeleteMemoCriteria : BaseCommand
    {
        public int Id { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Memo_Delete";
        }
    }
}