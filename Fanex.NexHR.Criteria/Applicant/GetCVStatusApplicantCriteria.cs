namespace Fanex.NexHR.Criteria.Applicant
{
    using Data.Repository;

    public class GetCVStatusApplicantCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Applicant_GetCVStatus";

        public override bool IsValid() => true;
    }
}
