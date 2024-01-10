namespace Fanex.NexHR.Criteria.Transfer
{
    using Fanex.Data.Repository;
    using System;

    public class GetTransferCriteria : CriteriaBase
    {
        public int? GroupId { get; set; }

        public int? EmpId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "Transfer_Get";

        public override bool IsValid() => ToDate >= FromDate;
    }
}
