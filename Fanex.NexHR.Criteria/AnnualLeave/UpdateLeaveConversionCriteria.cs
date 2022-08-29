namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using System;

    public class UpdateLeaveConversionCriteria : BaseCommand
    {
        public int LeaveConversionId { get; set; }

        public DateTime Date { get; set; }

        public decimal TotalDays { get; set; }

        public string Description { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "LeaveConversion_Update";
        }

        public override bool IsValid()
        {
            return LeaveConversionId > 0 && TotalDays > 0;
        }
    }
}