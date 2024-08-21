namespace Fanex.NexHR.Criteria.AnnualLeave
{
    using Data;
    using Data.Repository;

    public class GetAnnualLeaveTransactionCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public int Year { get; set; }

        [SpParam(SqlParamName = "excludedRequestTrans")]
        public bool ExcludedRequestTransactions { get; set; }

        public override string GetSettingKey() => "AnnualLeave_GetTransByEmp";

        public override bool IsValid() => EmpId > 0 && Year > 0;
    }
}
