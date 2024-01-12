namespace Fanex.NexHR.Criteria.HistoryWork
{
    using System;

    public class UpdateHistoryWorkCriteria : BaseCommand
    {
        public int Id { get; set; }

        public int EmpId { get; set; }

        public DateTime AffectedDate { get; set; }

        public int ModifiedBy { get; set; }

        public int JobTitleId { get; set; }

        public int GroupId { get; set; }

        public override string GetSettingKey()
        {
            return "HistoryWork_Update";
        }

        public override bool IsValid()
        {
            return Id > 0 && EmpId > 0;
        }
    }
}