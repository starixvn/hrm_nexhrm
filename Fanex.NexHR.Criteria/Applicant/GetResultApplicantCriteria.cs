namespace Fanex.NexHR.Criteria.Applicant
{
    using Data.Repository;

    public class GetResultApplicantCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Applicant_GetResult";

        public override bool IsValid() => true;
    }
}
