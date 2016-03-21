namespace Fanex.NexHR.Criteria.LateInEarlyOut
{
    using System;
    using Fanex.Data.Repository;

    public class GetAllEmployeeCriteria : CriteriaBase
    {
        public DateTime Date { get; set; }

        public override bool IsValid()
        {
            return Date != null;
        }

        public override string GetSettingKey()
        {
            return "LateInEarlyOut_GetAllEmployee";
        }
    }
}
