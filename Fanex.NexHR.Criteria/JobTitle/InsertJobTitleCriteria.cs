namespace Fanex.NexHR.Criteria.JobTitle
{
    public class InsertJobTitleCriteria : BaseCommand
    {
        public string Name { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "JobTitle_Insert";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Name);
    }
}
