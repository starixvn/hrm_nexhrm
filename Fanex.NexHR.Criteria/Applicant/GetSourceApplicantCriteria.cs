namespace Fanex.NexHR.Criteria.Applicant
{
    using Data.Repository;

    public class GetSourceApplicantCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Applicant_GetSource";

        public override bool IsValid() => true;
    }
}
