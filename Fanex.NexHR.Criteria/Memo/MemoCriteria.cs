namespace Fanex.NexHR.Criteria.Memo
{
    using Data;
    using Data.Repository;
    using System;

    public class MemoCriteria : CriteriaBase
    {
        public string RefNo { get; set; }

        public string Contents { get; set; }

        public string Subject { get; set; }

        public string SentTo { get; set; }

        public string SentFrom { get; set; }

        public string ForInfo { get; set; }

        public int TotalPages { get; set; }

        public DateTime SentDate { get; set; }

        public int Category { get; set; }

        public bool Status { get; set; }

        [SpParam(SqlParamName = "Type")]
        public int MemoType { get; set; }

        public override string GetSettingKey()
        {
            throw new NotImplementedException();
        }

        public override bool IsValid()
        {
            throw new NotImplementedException();
        }
    }
}
