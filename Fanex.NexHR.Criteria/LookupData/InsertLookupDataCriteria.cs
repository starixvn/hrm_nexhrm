namespace Fanex.NexHR.Criteria.LookupData
{
    public class InsertLookupDataCriteria : BaseCommand
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