namespace Fanex.NexHR.Criteria.LookupData
{
    using Fanex.Data.Repository;

    public class GetJobTitleCriteria : CriteriaBase
    {
        public string JobTitle { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Lookup_GetJobTitle";
        }
    }
}
