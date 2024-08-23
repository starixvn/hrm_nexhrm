namespace Fanex.NexHR.Criteria.Applicant
{
    using System;
    using Data.Repository;

    public class GetAllApplicantCriteria : CriteriaBase
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int Status { get; set; }

        public int PositionId { get; set; }

        public string FullName { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalRow { get; set; }

        public override string GetSettingKey() => "Applicant_Get";

        public override bool IsValid() => true;
    }
}
