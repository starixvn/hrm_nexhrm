namespace Fanex.NexHR.Criteria.LateInEarlyOut
{
    using System;
    using Data;
    using Data.Repository;

    public class UpdateLateInEarlyOutCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public DateTime WorkDate { get; set; }

        [SpParam(SqlParamName = "Type")]
        public int Types { get; set; }

        public int Minutes { get; set; }

        public int EmpId { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            var result = Types == 1 || Types == 2;

            return result;
        }

        public override string GetSettingKey()
        {
            return "LateInEarlyOut_Update";
        }
    }
}
