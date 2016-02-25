namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using System;
    using Fanex.Data.Repository;

    public class OpenAnnualBalanceForNewYearCriteria : CriteriaBase
    {
        public int Year { get; set; }

        public int? EmpId { get; set; }

        public string Description { get; set; }

        public override string GetSettingKey()
        {
            return "DBA_AnnualLeave_CreateAnnualBalance";
        }

        public override bool IsValid()
        {
            return Year > 2010;
        }
    }
}
