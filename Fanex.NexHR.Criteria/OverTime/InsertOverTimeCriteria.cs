namespace Fanex.NexHR.Criteria.Overtime
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models.Overtime;
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
            return TimeSheets.Any() && IsValidOvertimeDetail();
        }

        private bool IsValidOvertimeDetail()
        {
            bool isValid = true;
            int i = 0;
            while (isValid && i < TimeSheets.Count)
            {
                if (TimeSheets[i].Employee == null
                    || TimeSheets[i].Employee.Id <= 0
                    || TimeSheets[i].Hour < 1
                    || TimeSheets[i].Hour > 8)
                {
                    isValid = false;
                }

                i++;
            }

            return isValid;
        }
    }
}
