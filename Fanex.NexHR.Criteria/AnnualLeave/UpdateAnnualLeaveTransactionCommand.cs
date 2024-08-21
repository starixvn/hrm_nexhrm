namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using System;

    public class UpdateAnnualLeaveTransactionCommand : BaseCommand
    {
        public int LeaveTransactionId { get; set; }

        public string Description { get; set; }

        public DateTime FromDate { get; set; }

        public decimal DeductDays { get; set; }

        public decimal CreditDays { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "AnnualLeave_UpdateTransaction";

        public override bool IsValid() => LeaveTransactionId > 0;
    }
}