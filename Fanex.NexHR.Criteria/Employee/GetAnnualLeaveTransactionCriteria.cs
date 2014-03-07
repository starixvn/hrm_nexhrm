// -----------------------------------------------------------------------
// <copyright file="GetAnnualLeaveTransactionCriteria.cs" company="Nexcel Solutions Viet Nam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Get annual leave transaction criteria.
    /// </summary>
    public class GetAnnualLeaveTransactionCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpID")]
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
            return true;
        }
    }
}
