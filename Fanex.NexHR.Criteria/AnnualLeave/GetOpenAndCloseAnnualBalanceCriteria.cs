namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using System;
    using Data.Repository;

    public class GetOpenAndCloseAnnualBalanceCriteria : CriteriaBase
    {
        public DateTime OpenDate { get; set; }

        public DateTime CloseDate { get; set; }

        public override string GetSettingKey() => "AnnualLeave_GetOpenAndCloseBalance";

        public override bool IsValid() => true;

    }
}
