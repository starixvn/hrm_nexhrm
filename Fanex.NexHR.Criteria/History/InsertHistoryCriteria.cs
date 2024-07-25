namespace Fanex.NexHR.Criteria.History
{
    using System;

    public class InsertHistoryCriteria : BaseCommand
    {
        public int EmpId { get; set; }

        public int GroupId { get; set; }

        public DateTime AffectedDate { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "History_Insert";

        public override bool IsValid() => EmpId > 0 && GroupId > 0 && ModifiedBy > 0;
    }
}
