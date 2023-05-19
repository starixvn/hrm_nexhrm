namespace Fanex.NexHR.Criteria.Employee
{
    using Data;
    using Data.Repository;

    public class GetGeoMapInfoCriteria : CriteriaBase
    {
        public string Query { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "Employee_GetGeoMapEmployee";
        
    }
}
