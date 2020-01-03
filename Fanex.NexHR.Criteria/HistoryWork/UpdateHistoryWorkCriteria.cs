namespace Fanex.NexHR.Criteria.HistoryWork
{
    using Fanex.Data.Repository;
    using System;

    public class UpdateHistoryWorkCriteria : CriteriaBase
    {
        public int? Id { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string JobtitleName { get; set; }

        public string GroupName { get; set; }

        public override string GetSettingKey()
        {
            return "HistoryWork_Update";
        }

        public override bool IsValid()
        {
            return Id > 0;
        }
    }
}
