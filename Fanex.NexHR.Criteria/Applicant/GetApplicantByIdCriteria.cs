namespace Fanex.NexHR.Criteria.Team
{
    using Data.Repository;

    public class GetApplicantByIdCriteria : CriteriaBase
    {
        public int ApplicantId { get; set; }

        public override bool IsValid()
        {
            return ApplicantId > 0;
        }

        public override string GetSettingKey()
        {
            return "Applicant_GetById";
        }
    }
}
