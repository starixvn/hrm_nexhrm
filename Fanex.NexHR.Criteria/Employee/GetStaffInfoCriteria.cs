namespace Fanex.NexHR.Criteria.Employee
{
    using Data;
    using Data.Repository;

    public class GetStaffInfoCriteria : CriteriaBase
    {
        public string Query { get; set; }

        [SpParam(SqlParamName = "GroupId")]
        public int? TeamId { get; set; }

        public bool? IsSpecial { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "Employee_GetStaffInfo";
        
    }
}
