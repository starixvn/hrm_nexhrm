// --------------------------------------------------------------------------
// <copyright file="WorkingTimeRequestCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.7.31">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Request
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Working Time Request Criteria.
    /// </summary>
    public class WorkingTimeRequestCriteria : CriteriaBase
    {
        public int EmpId { get; set; }

        public int RequestType { get; set; }

        public bool IsPlanned { get; set; }

        public string Remark { get; set; }

        public DateTime FromDate { get; set; }

        public int? FromShift { get; set; }

        public DateTime? ToDate { get; set; }

        public int? ToShift { get; set; }

        public int? LeaveTypeId { get; set; }

        public decimal? TotalDays { get; set; }

        public int? LateInMins { get; set; }

        public int? EarlyOutMins { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public string ErrMsg { get; set; }

        public int ErrCode { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
