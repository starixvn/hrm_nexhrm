namespace Fanex.NexHR.Criteria.LookupData
{
    public class UpdateLookupDataCriteria : BaseCommand
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public override bool IsValid()
        {
            return Id > 0 && !string.IsNullOrWhiteSpace(Name);
        }

        public override string GetSettingKey()
        {
            return "LookupData_Update";
        }
    }
}