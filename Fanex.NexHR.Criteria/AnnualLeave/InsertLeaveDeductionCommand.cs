namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using System;

    public class InsertLeaveDeductionCommand : BaseCommand
    {
        public int EmployeeId { get; set; }

        public DateTime DeductDate { get; set; }

        public bool IsAnnual { get; set; }

        public decimal TotalDays { get; set; }

        public string Description { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "LeaveDeduction_Insert";

        public override bool IsValid() => EmployeeId > 0 && TotalDays > 0;
    }
}