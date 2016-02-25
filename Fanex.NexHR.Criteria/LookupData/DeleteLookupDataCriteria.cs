namespace Fanex.NexHR.Criteria.LookupData
{
    using Fanex.Data.Repository;

    public class DeleteLookupDataCriteria : CriteriaBase
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
