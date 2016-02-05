namespace Fanex.NexHR.Criteria.Overtime
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class UpdateOvertimeCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int ModifiedBy { get; set; }

        public string Description { get; set; }

        public int Status { get; set; }

        [SpParam(Ignored = true)]
        public IList<OvertimeRecord> TimeSheets { get; set; }

        public string OvertimeSheet
        {
            get
            {
                var time = TimeSheets.Select(i => string.Format("{0}:{1}", i.EmployeeId, i.Hour));
                var times = string.Join(",", time);

                return times;
            }
        }

        public override string GetSettingKey()
        {
            return "Overtime_Update";
        }

        public override bool IsValid()
        {
            return TimeSheets.Count > 0;
        }
    }
}
