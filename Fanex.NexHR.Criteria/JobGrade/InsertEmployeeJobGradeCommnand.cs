namespace Fanex.NexHR.Criteria.JobGrade
{
    using System;

    public class InsertEmployeeJobGradeCommnand : BaseCommand
    {
        public int EmpId { get; set; }

        public int JobGradeId { get; set; }

        public int OldJobGradeId { get; set; }

        public bool IsPromotion { get; set; }

        public string Note { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime EffectiveDate { get; set; }

        public override string GetSettingKey() => "EmployeeJobGrade_Insert";

        public override bool IsValid() => EmpId > 0 && ModifiedBy > 0 && IsValidJobGrade();

        private bool IsValidJobGrade() => JobGradeId > 0 && JobGradeId != OldJobGradeId;
    }
}
