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

        //public int? Enable => EnableStatus.HasValue ? (EnableStatus.Value ? 0 : 1) : new int?();     
        public int? Enable
        {
            get
            {
                if (EnableStatus.HasValue)
                {
                    if (EnableStatus.Value)
                    {
                        return 0;
                    }

                    return 1;
                }

                return new int?();
            }
        }

        public int? IsDelete
        {
            get
            {
                if (DeleteStatus.HasValue)
                {
                    if (DeleteStatus.Value)
                    {
                        return 0;
                    }

                    return 1;
                }

                return new int?();
            }
        }

        //public int? IsDelete => DeleteStatus.HasValue ? (DeleteStatus.Value ? 1 : 0) : new int?();

        public int? JobTitleId { get; set; }

        [SpParam(Ignored = true)]
        public bool? EnableStatus { get; set; }

        [SpParam(Ignored = true)]
        public bool? DeleteStatus { get; set; }

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
