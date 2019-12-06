namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using Data.Repository;

    public class GetAnnualLeaveSummaryCriteria : CriteriaBase
    {
        public int? EmployeeId { get; set; }

        public int? GroupId { get; set; }

        public int Year { get; set; }

        public int MinAnnualBalance { get; set; }

        public override string GetSettingKey()
        {
            return "AnnualLeave_GetSummary";
        }

        public override bool IsValid()
        {
            const short minValidYear = 2010;
            return Year >= minValidYear;
        }
    }
}
