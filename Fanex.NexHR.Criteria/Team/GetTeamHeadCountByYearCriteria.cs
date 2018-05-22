namespace Fanex.NexHR.Criteria.Team
{
    using System;
    using Data.Repository;

    public class GetHeadCountByYearCriteria : CriteriaBase
    {
        public DateTime StartDateOfYear { get; set; }

        public DateTime EndDateOfYear { get; set; }

        public override string GetSettingKey() => "EmployeeGroup_GetHeadCountByYear";

        public override bool IsValid() => EndDateOfYear.Year <= DateTime.Now.Year && StartDateOfYear < EndDateOfYear;
    }
}