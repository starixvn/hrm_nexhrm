namespace Fanex.NexHR.Criteria.History
{
    using System;

    public class InsertHistoryCriteria : BaseCommand
    {
        public int EmpId { get; set; }

        public int? GroupId { get; set; }

        public int? JobTitleId { get; set; }

        public int? JobGradeId { get; set; }

        public DateTime AffectedDate { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "History_Insert";

        private bool IsValidGroupJobTitle() => GroupId > 0 || JobTitleId > 0 || JobGradeId > 0;

        public override bool IsValid() => EmpId > 0 && IsValidGroupJobTitle() && ModifiedBy > 0;
    }
}