namespace Fanex.NexHR.Criteria.Memo
{
    using Data.Repository;

    /// <summary>
    /// Get Memo By Id.
    /// </summary>
    public class GetMemoByIdCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Memo_GetById";
        }
    }
}
