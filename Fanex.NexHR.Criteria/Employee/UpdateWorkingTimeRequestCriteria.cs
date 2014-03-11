// --------------------------------------------------------------------------
// <copyright file="UpdateWorkingTimeRequestCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.3.10">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Update Working Time Request Criteria.
    /// </summary>
    public class UpdateWorkingTimeRequestCriteria : CriteriaBase
    {
        public long Id { get; set; }

        public bool IsPlanned { get; set; }

        public string Remark { get; set; }

        public DateTime FromDate { get; set; }

        public int? FromShift { get; set; }

        public DateTime? ToDate { get; set; }

        public int? ToShift { get; set; }

        public int? LeaveTypeID { get; set; }

        public decimal? TotalDays { get; set; }

        public int? LateInMins { get; set; }

        public int? EarlyOutMins { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_Update";
        }
    }
}
