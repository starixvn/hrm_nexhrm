using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fanex.NexHR.Criteria.LateInEarlyOut
{
    public class GetLateInEarlyOutByEmpIdCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "GetLateInEarlyOutByEmpId";
        }
    }
}
