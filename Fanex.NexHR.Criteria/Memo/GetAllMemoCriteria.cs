namespace Fanex.NexHR.Criteria.Memo
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get All Memo Criteria.
    /// </summary>
    public class GetAllMemoCriteria : CriteriaBase
    {
        public int Year { get; set; }

        public int Category { get; set; }

        public int Status { get; set; }

        public string Content { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Memo_GetAll";
        }
    }
}
