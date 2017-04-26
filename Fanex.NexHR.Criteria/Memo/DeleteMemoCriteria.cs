namespace Fanex.NexHR.Criteria.Memo
{
    using Data.Repository;

    /// <summary>
    /// Delete Memo Criteria.
    /// </summary>
    public class DeleteMemoCriteria : CriteriaBase
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
