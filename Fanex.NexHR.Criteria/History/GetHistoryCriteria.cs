namespace Fanex.NexHR.Criteria.History
{
    using Fanex.Data.Repository;
    using System;

    public class GetHistoryCriteria : CriteriaBase
    {
        public int? GroupId { get; set; }

        public int? EmpId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "History_Get";
        public override bool IsValid() => ToDate >= FromDate;
    }
}
