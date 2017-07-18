namespace Fanex.NexHR.Criteria.Memo
{
    /// <summary>
    /// Update Memo Criteria.
    /// </summary>
    public class UpdateMemoCriteria : MemoCriteria
    {
        public int Id { get; set; }

        public override string GetSettingKey()
        {
            return "Memo_Update";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}