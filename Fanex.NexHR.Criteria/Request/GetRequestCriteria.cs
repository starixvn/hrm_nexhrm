// --------------------------------------------------------------------------
// <copyright file="GetRequestCriteria.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.7.31">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Request
{
    using System;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Core.Attributes;

    /// <summary>
    /// Get Request Criteria.
    /// </summary>
    public class GetRequestCriteria : CriteriaBase
    {
        public int Role { get; set; }

        [SpParam(SqlParamName = "empid")]
        public int? EmployeeId { get; set; }

        public int? GroupId { get; set; }

        public int? LeaveTypeId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int? Status { get; set; }

        public int ActorId { get; set; }

        [SpParam(SqlParamName = "excludedEmpID")]
        public int? ExcludeEmployeeId { get; set; }

        public string RequestType { get; set; }

        public bool IsFilterByEffectedDate { get; set; }

        public int? PageIndex { get; set; }

        public int? ItemsPerPage { get; set; }

        [Output]
        public int? TotalItem { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_GetRequestByDate";
        }

        public override bool IsValid()
        {
            return FromDate <= ToDate;
        }
    }
}