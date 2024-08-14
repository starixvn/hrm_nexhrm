namespace Fanex.NexHR.Criteria.LookupData
{
    using Data.Repository;

    public class GetAllLookupDataCriteria : CriteriaBase
    {
        public string LookupCode { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "Lookup_GetLookupData";
    }
}
