namespace Fanex.NexHR.Criteria.JobTitle
{
    public class UpdateJobTitleCriteria : BaseCommand
    {
        public int JobTitleId { get; set; }

        public string Name { get; set; }

        public override bool IsValid() => JobTitleId > 0 && !string.IsNullOrWhiteSpace(Name);

        public override string GetSettingKey() => "JobTitle_Update";
    }
}