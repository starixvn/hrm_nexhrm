namespace Fanex.NexHR.Criteria.Applicant
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
