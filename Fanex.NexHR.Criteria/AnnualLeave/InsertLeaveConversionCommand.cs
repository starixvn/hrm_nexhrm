namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using System;

    public class InsertLeaveConversionCommand : BaseCommand
    {
        public int EmployeeId { get; set; }

        public DateTime Date { get; set; }

        public decimal TotalDays { get; set; }

        public string Description { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "LeaveConversion_Insert";

        public override bool IsValid() => EmployeeId > 0 && TotalDays > 0;
    }
}