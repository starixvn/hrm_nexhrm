// --------------------------------------------------------------------------
// <copyright file="UpdateRequestCriteria.cs" company=".">
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

    /// <summary>
    /// Update Request Criteria.
    /// </summary>
    public class UpdateRequestCriteria : CriteriaBase
    {
        public long Id { get; set; }

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

        public DateTime ModifiedDate { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "WorkingTimeRequest_Update";
        }

        public override bool IsValid()
        {
            return Id > 0;
        }
    }
}
