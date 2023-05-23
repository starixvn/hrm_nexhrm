namespace Fanex.NexHR.Criteria.TrainingAndDevelopment
{
    public class DeleteTrainingAndDevelopmentCriteria : BaseCommand
    {
        public int Id { get; set; }
        public override string GetSettingKey() => "TrainingAndDevelopment_Delete";

        public override bool IsValid() => Id > 0;
    }
}
