namespace Fanex.NexHR.Criteria.LateInEarlyOut
{
    using Fanex.Data.Repository;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class GetLateInEarlyOutByEmpIdCriteria : CriteriaBase
    {
        public int EmpId { get; set; }        

        public int Year { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "LateInEarlyOut_GetAllByEmpId";
        }
    }
}
