namespace Fanex.NexHR.Criteria.HistoryWork
{
    using System;

    public class UpdateHistoryWorkCriteria : BaseCommand
    {
        public int? Id { get; set; }

        public DateTime AffectedDate { get; set; }

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