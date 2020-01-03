namespace Fanex.NexHR.Criteria.HistoryWork
{
    using Fanex.Data.Repository;
    using System;

    public class InsertHistoryWorkCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public int? HistoryId { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public DateTime AffectedDate { get; set; }

        public string JobtitleName { get; set; }

        public string GroupName { get; set; }

        public override string GetSettingKey()
        {
            return "HistoryWork_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
