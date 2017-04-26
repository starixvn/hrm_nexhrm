namespace Fanex.NexHR.Criteria.LookupData
{
    using Data.Repository;

    public class InsertLookupDataCriteria : CriteriaBase
    {
        public string Name { get; set; }

        public string LookupCode { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Name);
        }

        public override string GetSettingKey()
        {
            return "LookupData_Insert";
        }
    }
}
