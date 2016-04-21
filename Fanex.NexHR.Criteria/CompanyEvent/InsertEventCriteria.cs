namespace Fanex.NexHR.Criteria.CompanyEvent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Data.Repository;

    public class InsertEventCriteria : CriteriaBase
    {
        public string Title { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string FromTime { get; set; }

        public string ToTime { get; set; }

        public string Description { get; set; }

        public int CreatedBy { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "CompanyEvent_Insert";
        }
    }
}
