namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using System;

    public class InsertLeaveDeductionCriteria : BaseCommand
    {
        public int EmployeeId { get; set; }

        public DateTime DeductDate { get; set; }

        public bool IsAnnual { get; set; }

        public decimal TotalDays { get; set; }

        public string Description { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey()
        {
            return "LeaveDeduction_Insert";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0 && TotalDays > 0;
        }
    }
}