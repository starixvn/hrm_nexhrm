namespace Fanex.NexHR.Criteria.Applicant
{
    using Data.Repository;

    public class GetProLevelApplicantCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Applicant_GetProfessionalLevel";

        public override bool IsValid() => true;
    }
}
