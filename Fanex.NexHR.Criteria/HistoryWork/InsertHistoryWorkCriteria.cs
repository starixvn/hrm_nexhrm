namespace Fanex.NexHR.Criteria.HistoryWork
{
    using System;

    public class InsertHistoryWorkCriteria : BaseCommand
    {
        public int EmpId { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime AffectedDate { get; set; }

        public int JobTitleId { get; set; }

        public int GroupId { get; set; }

        public override string GetSettingKey()
        {
            return "HistoryWork_Insert";
        }

        public override bool IsValid()
        {
            return EmpId > 0 && GroupId > 0 && JobTitleId > 0;
        }
    }
}