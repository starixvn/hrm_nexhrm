using Fanex.Data.Repository;
using Fanex.NexHR.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fanex.NexHR.Criteria.OverTime
{
    public class GetByIdCriteria : CriteriaBase
    {        
        public int Id { get; set; }     

        public override string GetSettingKey()
        {
            return "OverTime_GetById";
        }
    }
}
