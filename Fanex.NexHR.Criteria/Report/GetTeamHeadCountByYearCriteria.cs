namespace Fanex.NexHR.Criteria.Report
{
    using System;
    using Data.Repository;
    using Fanex.NexHR.Core;

    public class GetHeadCountByYearCriteria : CriteriaBase
    {
        public DateTime StartDateOfYear { get; set; }

        public DateTime EndDateOfYear { get; set; }

        public override string GetSettingKey() => "Report_GetHeadCountByYear";

        public override bool IsValid() => EndDateOfYear.Year <= DateTimeOffset.Now.DateTime.ToClientTimeZone().Year && StartDateOfYear < EndDateOfYear;
    }
}