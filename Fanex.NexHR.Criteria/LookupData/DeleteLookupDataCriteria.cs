namespace Fanex.NexHR.Criteria.LookupData
{
    public class DeleteLookupDataCriteria : BaseCommand
    {
        public int Id { get; set; }

        public override bool IsValid()
        {
            return Id > 0;
        }

        public override string GetSettingKey()
        {
            return "LookupData_Delete";
        }
    }
}