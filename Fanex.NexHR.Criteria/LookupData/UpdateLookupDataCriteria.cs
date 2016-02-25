namespace Fanex.NexHR.Criteria.LookupData
{
    using Fanex.Data.Repository;

    public class UpdateLookupDataCriteria : CriteriaBase
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
