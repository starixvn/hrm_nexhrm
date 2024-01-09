namespace Fanex.NexHR.Criteria.JobGrade
{
    using System;

    public class UpdateJobGradeCriteria : BaseCommand
    {

        public int EmpId { get; set; }

        public int JobGradeId { get; set; }

        public string Note { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime EffectiveDate { get; set; }

        public override string GetSettingKey() => "JobGrade_Insert";

        public override bool IsValid() => ModifiedBy > 0 && JobGradeId > 0;
    }
}
