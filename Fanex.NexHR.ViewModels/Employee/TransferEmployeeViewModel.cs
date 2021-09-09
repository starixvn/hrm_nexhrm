namespace Fanex.NexHR.ViewModels.Employee
{
    using Fanex.NexHR.Models;
    using System;
    using System.Collections.Generic;

    public class TransferEmployeeViewModel
    {
        public int EmployeeId { get; set; }

        public string Employee { get; set; }

        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public int ShiftId { get; set; }

        public string ShiftName { get; set; }

        public DateTime LastWorkingDate { get; set; }

        public DateTime AffectedDate { get; set; }

        public bool IsTransferLateInEarlyOut { get; set; }

        public bool IsTransferAnnualLeaveBalance { get; set; }
        
        public int ModifiedBy { get; set; }

        public IList<LuminetGroup> LumiGroups { get; set; }

        public IList<LuminetShift> LumiShifts { get; set; }
    }
}
