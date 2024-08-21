namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using System;

    public class UpdateLeaveDeductionCommand : BaseCommand
    {
        public int LeaveDeductionId { get; set; }

        public DateTime DeductDate { get; set; }

        public bool IsAnnual { get; set; }

        public decimal TotalDays { get; set; }

        public string Description { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "LeaveDeduction_Update";

        public override bool IsValid() => LeaveDeductionId > 0 && TotalDays > 0;
    }
}