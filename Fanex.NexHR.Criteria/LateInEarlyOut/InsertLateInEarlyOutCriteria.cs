namespace Fanex.NexHR.Criteria.LateInEarlyOut
{
    using System;
    using Data.Repository;

    public class InsertLateInEarlyOutCriteria : CriteriaBase
    {
        public DateTime WorkDate { get; set; }
        
        /// <summary>
        /// 1: Late In or 2: Early Out
        /// </summary>
        [SpParam(SqlParamName = "Type")]
        public int Types { get; set; }

        public int Minutes { get; set; }

        public int EmpId { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            bool result;
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
            return "LateInEarlyOut_Insert";
        }
    }
}
