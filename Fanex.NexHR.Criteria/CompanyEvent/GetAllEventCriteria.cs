namespace Fanex.NexHR.Criteria.CompanyEvent
{
    using System;
    using Data.Repository;

    public class GetAllEventCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "CompanyEvent_GetAll";
    }
}
