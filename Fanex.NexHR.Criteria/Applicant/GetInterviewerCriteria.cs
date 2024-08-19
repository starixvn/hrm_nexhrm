namespace Fanex.NexHR.Criteria.Applicant
{
    using Data.Repository;

    public class GetInterviewerCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Applicant_GetInterViewer";

        public override bool IsValid() => true;
    }
}
