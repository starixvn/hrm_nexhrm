using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fanex.NexHR.Criteria.OverTime
{
    public class UpdateStatusCriteria : CriteriaBase
    {
        public int Status { get; set; }
        public int Id { get; set; }

        public override string GetSettingKey()
        {
            return "OverTime_UpdateStatus";
        }
    }
}
