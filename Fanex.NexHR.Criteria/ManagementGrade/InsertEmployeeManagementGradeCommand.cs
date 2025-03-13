namespace Fanex.NexHR.Criteria.ManagementGrade
{
    using System;

    public class InsertEmployeeManagementGradeCommand : BaseCommand
    {
        public int EmpId { get; set; }

        public int ManagementGradeId { get; set; }

        public int OldManagementGradeId { get; set; }

        public bool IsPromotion { get; set; }

        public string Note { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime EffectiveDate { get; set; }

        public override string GetSettingKey() => "EmployeeManagementGrade_Insert";

        public override bool IsValid() => EmpId > 0 && ModifiedBy > 0 && IsValidJobGrade();

        private bool IsValidJobGrade() => ManagementGradeId > 0 && ManagementGradeId != OldManagementGradeId;
    }
}
