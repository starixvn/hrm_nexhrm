namespace Fanex.NexHR.Criteria.Promotion
{
    using System;

    public class UpdatePromotionCriteria : BaseCommand
    {
        public int Id { get; set; }

        public int JobGradeId { get; set; }

        public int OldJobGradeId { get; set; }

        public DateTime EffectiveDate { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Promotion_Update";

        public override bool IsValid() => ModifiedBy > 0 && IsValidJobGrade();

        private bool IsValidJobGrade() => JobGradeId > 0 && OldJobGradeId > 0 && JobGradeId != OldJobGradeId ;
    }
}
