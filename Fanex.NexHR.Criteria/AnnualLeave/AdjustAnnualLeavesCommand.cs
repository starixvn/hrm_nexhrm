namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using System;
    using Data;

    public class AdjustAnnualLeavesCommand : BaseCommand
    {
        [SpParam(SqlParamName = "EmpIds")]
        public string EmployeeId { get; set; }

        public string Description { get; set; }

        public DateTime FromDate { get; set; }

        public decimal DeductDays { get; set; }

        public decimal CreditDays { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "AnnualLeave_InsertMultipleTransactions";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(EmployeeId);
    }
}