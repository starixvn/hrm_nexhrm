namespace Fanex.NexHR.Criteria.JobTitle
{
    public class DeleteJobTitleCriteria : BaseCommand
    {
        public int JobTitleId { get; set; }

        public override bool IsValid() => JobTitleId > 0;

        public override string GetSettingKey() => "JobTitle_Delete";
    }
}