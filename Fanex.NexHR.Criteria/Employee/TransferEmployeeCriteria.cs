namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data.Repository;
    using System;

    public class TransferEmployeeCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public DateTime LastWorkingDate { get; set; }

        public DateTime AffectedDate { get; set; }

        public int ToGroupId { get; set; }

        public int ShiftId { get; set; }

        public int IsTransferLateInEarlyOut { get; set; }
        
        public int IsTransferAnnualLeaveBalance { get; set; }

        public int ModifiedBy { get; set; }

        public int IsTransferNexBalance { get; set; }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }

        public override string GetSettingKey()
        {
            return "Employee_Transfer";
        }
    }
}
