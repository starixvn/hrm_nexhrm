// -----------------------------------------------------------------------
// <copyright file="GetAnnualLeaveTransactionCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.03.10">Changed</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using Fanex.Data.Repository;

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
