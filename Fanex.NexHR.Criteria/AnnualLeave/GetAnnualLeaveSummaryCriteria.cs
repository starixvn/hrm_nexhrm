namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using Data.Repository;

    public class GetAnnualLeaveSummaryCriteria : CriteriaBase
    {
        const short minValidYear = 2010;

        public int? EmployeeId { get; set; }

        public int? GroupId { get; set; }

        public int? DepartmentId { get; set; }

        public int Year { get; set; }

        public int MinAnnualBalance { get; set; }

        public override string GetSettingKey() => "AnnualLeave_GetSummary";

        public override bool IsValid() => Year >= minValidYear;
    }
}
