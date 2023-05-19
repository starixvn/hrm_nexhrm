namespace Fanex.NexHR.Criteria.TrainingAndDevelopment
{
    using Fanex.Data.Repository;

    public class GetTrainingAndDevelopmentByIdCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public override string GetSettingKey() => "CourseAndCertificate_GetById";

        public override bool IsValid() => Id > 0;
    }
}
