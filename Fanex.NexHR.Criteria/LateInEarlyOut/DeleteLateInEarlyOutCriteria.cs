using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fanex.NexHR.Criteria.LateInEarlyOut
{
    public class DeleteLateInEarlyOutCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "DeleteLateInEarlyOut";
        }
    }
}
