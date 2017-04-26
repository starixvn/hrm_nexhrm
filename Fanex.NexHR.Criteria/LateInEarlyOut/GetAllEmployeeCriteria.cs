namespace Fanex.NexHR.Criteria.LateInEarlyOut
{
    using Data.Repository;
    using System;

    public class GetAllEmployeeCriteria : CriteriaBase
    {
        public DateTime Date { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "LateInEarlyOut_GetAllEmployee";
        }
    }
}
