namespace Fanex.NexHR.Criteria.Memo
{
    using System;
    using Core.Attributes;
    using Data.Repository;

    /// <summary>
    /// Insert Memo Criteria.
    /// </summary>
    public class InsertMemoCriteria : CriteriaBase
    {
        [Output]
        public int MemoId { get; set; }

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

        [SpParam(Ignored = true)]
        public bool SendMail { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Memo_Insert";
        }
    }
}
