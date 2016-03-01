namespace Fanex.NexHR.Criteria.LateInEarlyOut
{
    using Fanex.Data.Repository;
    using System;    
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class UpdateLateInEarlyOutCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public DateTime WorkDate { get; set; }

        [SpParam(SqlParamName = "Type")]
        public int Types { get; set; }

        public int Minutes { get; set; }

        public int EmpId { get; set; }

        public int ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public override bool IsValid()
        {
            bool result = false;
            if (Types == 1 || Types == 2)
            {
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }

        public override string GetSettingKey()
        {
            return "UpdateLateInEarlyOut";
        }
    }
}
