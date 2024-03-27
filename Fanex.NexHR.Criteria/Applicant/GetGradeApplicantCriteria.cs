namespace Fanex.NexHR.Criteria.Applicant
{
    using Data.Repository;

    public class GetGradeApplicantCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Applicant_GetGrade";

        public override bool IsValid() => true;
    }
}
