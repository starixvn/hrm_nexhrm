namespace Fanex.NexHR.Criteria.History
{
    using System;

    public class UpdateHistoryCriteria : BaseCommand
    {
        public int Id { get; set; }

        public int EmpId { get; set; }

        public int? GroupId { get; set; }

        public int? JobTitleId { get; set; }

        public int? JobGradeId { get; set; }

        public DateTime AffectedDate { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "History_Update";

        public override bool IsValid() => Id > 0 && ModifiedBy > 0 && IsValidGroup();

        private bool IsValidGroup() => GroupId > 0 || JobTitleId > 0 || JobGradeId > 0;
    }
}
