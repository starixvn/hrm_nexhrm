namespace Fanex.NexHR.Criteria.CompanyEvent
{
    using System;
    using Fanex.Data.Repository;

    public class UpdateEventCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string FromTime { get; set; }

        public string ToTime { get; set; }

        public string Description { get; set; }

        public override bool IsValid()
        {
            return Id > 0;
        }

        public override string GetSettingKey()
        {
            return "CompanyEvent_Update";
        }
    }
}
