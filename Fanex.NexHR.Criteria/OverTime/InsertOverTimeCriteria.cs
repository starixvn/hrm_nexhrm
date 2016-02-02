namespace Fanex.NexHR.Criteria.Overtime
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class InsertOvertimeCriteria : CriteriaBase
    {
        public DateTime Date { get; set; }

        public int CreatedBy { get; set; }

        public string Description { get; set; }

        public int Status { get; set; }
        
        [SpParam(Ignored = true)]
        public IList<OvertimeRecord> OvertimeRecord { get; set; }

        public override string GetSettingKey()
        {
            return "Overtime_Insert";
        }

        public override Hashtable GetSpParams()
        {
            var hashTable = base.GetSpParams();
            if (!hashTable.ContainsKey("overtimesheet"))
            {
                var overTimeSheets = OvertimeRecord.Select(i => string.Format("{0}:{1}", i.EmployeeId, i.Hour));
                hashTable["overtimesheet"] = string.Join(",", overTimeSheets);
            }

            return hashTable;
        }
    }
}
