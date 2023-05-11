namespace Fanex.NexHR.Criteria.CourseAndCertificate
{
    using Fanex.Data.Repository;

    public class GetTrainingAndDevelopmentType : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "TrainingAndDevelopmentType_Get";
        }

        public override bool IsValid() => true;
    }
}
