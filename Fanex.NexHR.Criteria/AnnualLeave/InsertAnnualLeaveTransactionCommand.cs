namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using System;

    public class InsertAnnualLeaveTransactionCommand : BaseCommand
    {
        public int EmployeeId { get; set; }

        public int? RequestId { get; set; }

        public string Description { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public decimal DeductDays { get; set; }

        public decimal CreditDays { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "AnnualLeave_InsertTransaction";

        public override bool IsValid() => true;
    }
}