using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fanex.NexHR.Criteria.OverTime
{
    public class ApproveRequestCriteria : CriteriaBase
    {
        public int Status { get; set; }
        public int Id { get; set; }
        public int ApprovedUserId { get; set; }
        public override string GetSettingKey()
        {
            return "OverTime_ApproveRequest";
        }        
    }
}
