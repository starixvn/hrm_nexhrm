using Fanex.Data.Repository;
using Fanex.NexHR.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fanex.NexHR.Criteria.OverTime
{
    public class InsertOverTimeCriteria : CriteriaBase
    {
        public DateTime Date { get; set; }
        public int CreatedBy { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        
        [SpParam(Ignored = true)]
        public IList<OverTimeRecord> OverTimeRecord { get; set; }

        public override string GetSettingKey()
        {
            return "OverTime_Insert";
        }

        public override Hashtable GetSpParams()
        {
            var hashTable = base.GetSpParams();
            if (!hashTable.ContainsKey("overtimesheet"))
            {
                var overTimeSheets = OverTimeRecord.Select(i => string.Format("{0}:{1}", i.EmployeeId, i.Hour));
                hashTable["overtimesheet"] = string.Join(",", overTimeSheets);
            }

            return hashTable;
        }
    }
}
