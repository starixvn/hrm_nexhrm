namespace Fanex.NexHR.Criteria.Promotion
{
    using System;

    public class InsertPromotionCriteria : BaseCommand
    {
        public int EmpId { get; set; }

        public int JobGradeId { get; set; }

        public int OldJobGradeId { get; set; }

        public DateTime EffectiveDate { get; set; }

        public int ModifiedBy { get; set; }

        public string Note { get; set; }

        public override string GetSettingKey() => "Promotion_Insert";

        public override bool IsValid() => EmpId > 0 && ModifiedBy > 0 && IsValidJobGrade();

        private bool IsValidJobGrade() => JobGradeId > 0 && OldJobGradeId > 0 && JobGradeId != OldJobGradeId ;
    }
}
