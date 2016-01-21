using Fanex.Data.Repository;
using Fanex.NexHR.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fanex.NexHR.Criteria.OverTime
{
    public class GetSummaryCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int CreatedBy { get; set; }
        public int EmployeeId { get; set; }
        public int Status { get; set; }

        public override string GetSettingKey()
        {
            return "OverTime_GetSummary";
        }
    }
}
