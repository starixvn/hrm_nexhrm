namespace Fanex.NexHR.Criteria.Promotion
{
    using Fanex.Data.Repository;
    using System;

    public class GetPromotionCriteria : CriteriaBase
    {
        public int? GroupId { get; set; }

        public int? EmpId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "Promotion_Get";

        public override bool IsValid() => ToDate >= FromDate;
    }
}
