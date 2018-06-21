namespace Fanex.NexHR.Criteria.RedmineAPI
{
    using System;
    using Core.Enums;
    using Data.Repository;

    public class WriteLogInputSpendTimeRedmineCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public int IssueId { get; set; }

        public DateTime SpentOn { get; set; }

        public double? Hours { get; set; }

        public string Comments { get; set; }

        public RedmineLogTimeActivity Activity { get; set; }

        public DateTime CreatedDate { get; set; }

        public override string GetSettingKey() => "RedmineSpendTimeWriteLog_Insert";

        public override bool IsValid() => EmpId > 0;
    }
}