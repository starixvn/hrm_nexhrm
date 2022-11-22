namespace Fanex.NexHR.Criteria.Employee
{
    using Data;
    using Data.Repository;

    public class GetEmployeeCriteria : CriteriaBase
    {
        public string Query { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalRow { get; set; }

        [SpParam(SqlParamName = "GroupId")]
        public int? TeamId { get; set; }

        public bool? IsDisable { get; set; }

        public bool? IsDelete { get; set; }

        public int? JobTitleId { get; set; }

        public bool? IsSpecial { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Employee_GetEmployees";
        }
    }
}
