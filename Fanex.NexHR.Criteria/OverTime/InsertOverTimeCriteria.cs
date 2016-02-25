namespace Fanex.NexHR.Criteria.Overtime
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class InsertOvertimeCriteria : CriteriaBase
    {
        public DateTime Date { get; set; }

        public int CreatedBy { get; set; }

        public string Description { get; set; }

        public int Status { get; set; }

        [SpParam(Ignored = true)]
        public IList<OvertimeRecord> TimeSheets { get; set; }

        public string OvertimeSheet
        {
            get
            {
                var time = TimeSheets.Select(i => string.Format("{0}:{1}", i.Employee.Id, i.Hour));
                var times = string.Join(",", time);

                return times;
            }
        }

        public override string GetSettingKey()
        {
            return "Overtime_Insert";
        }

        public override bool IsValid()
        {
            return TimeSheets.Count > 0;
        }
    }
}
