using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fanex.NexHR.Criteria.OverTime
{
    public class UpdateOverTimeCriteria : CriteriaBase
    {
        public string Description { get; set; }

        public DateTime FromDate { get; set; }

        public float Hour { get; set; }

        public int EmployeeId { get; set; }

        public override string GetSettingKey()
        {
            return "OverTime_Update";
        }
    }
}
