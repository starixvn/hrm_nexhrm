using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fanex.NexHR.Criteria.OverTime
{
    public class VerifyRequestCriteria : CriteriaBase
    {
        public int Status { get; set; }
        public int Id { get; set; }
        public int VerifiedUserId { get; set; }
        public override string GetSettingKey()
        {
            return "OverTime_VerifyRequest";
        }        
    }
}
