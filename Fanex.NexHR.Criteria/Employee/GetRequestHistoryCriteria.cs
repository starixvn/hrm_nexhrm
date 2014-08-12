// -----------------------------------------------------------------------
// <copyright file="GetRequestHistoryCriteria.cs" company="Nexcel Solutions Viet Nam">
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
    /// Get request history criteria.
    /// </summary>
    public class GetRequestHistoryCriteria : CriteriaBase
    {
        public long RequestId { get; set; }

        [SpParam(SqlParamName = "Type")]
        public int RequestType { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_GetRequestHistory";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
