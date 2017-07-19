namespace Fanex.NexHR.Criteria.Memo
{
    using Core.Attributes;
    using Data;

    /// <summary>
    /// Insert Memo Criteria.
    /// </summary>
    public class InsertMemoCriteria : MemoCriteria
    {
        [Output]
        public int MemoId { get; set; }

        [SpParam(Ignored = true)]
        public bool SendMail { get; set; }

        public override string GetSettingKey()
        {
            return "Memo_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}