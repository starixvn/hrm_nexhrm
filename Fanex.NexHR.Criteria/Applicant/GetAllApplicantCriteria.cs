namespace Fanex.NexHR.Criteria.Applicant
{
    using System;
    using Data.Repository;

    public class GetAllApplicantCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int Status { get; set; }

        public int PositionId { get; set; }

        public string FullName { get; set; }

        public override string GetSettingKey()
        {
            return "Applicant_Get";
        }

        public override bool IsValid()
        {
            return FromDate <= ToDate;
        }
    }
}
