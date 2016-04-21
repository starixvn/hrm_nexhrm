namespace Fanex.NexHR.Criteria.CompanyEvent
{
    using System;
    using Fanex.Data.Repository;

    public class GetAllEventCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "CompanyEvent_GetAll";
        }
    }
}
