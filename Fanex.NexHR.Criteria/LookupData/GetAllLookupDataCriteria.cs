namespace Fanex.NexHR.Criteria.LookupData
{
    using Fanex.Data.Repository;

    public class GetAllLookupDataCriteria : CriteriaBase
    {
        public string LookupCode { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Lookup_GetLookupData";
        }
    }
}
