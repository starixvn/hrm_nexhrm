namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using System;
    using Fanex.Data.Repository;

    public class GetOpenAndCloseAnnualBalanceCriteria : CriteriaBase
    {
        public DateTime OpenDate { get; set; }

        public DateTime CloseDate { get; set; }

        public override string GetSettingKey()
        {
            return "AnnualLeave_GetOpenAndCloseBalance";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
