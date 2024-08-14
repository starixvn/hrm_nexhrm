namespace Fanex.NexHR.Criteria.JobTitle
{
    using Data.Repository;

    public class GetJobTitleCriteria : CriteriaBase
    {
        public string JobTitle { get; set; }

        public override string GetSettingKey() => "JobTitle_Get";

        public override bool IsValid() => true;
    }
}
