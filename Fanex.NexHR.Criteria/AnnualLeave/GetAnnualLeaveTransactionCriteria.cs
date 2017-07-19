namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using Data;
    using Data.Repository;

    public class GetAnnualLeaveTransactionCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public int Year { get; set; }

        [SpParam(SqlParamName = "excludedRequestTrans")]
        public bool ExcludedRequestTransactions { get; set; }

        public override string GetSettingKey()
        {
            return "AnnualLeave_GetTransByEmp";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0 && Year > 0;
        }
    }
}
