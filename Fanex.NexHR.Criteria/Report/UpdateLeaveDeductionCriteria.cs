// --------------------------------------------------------------------------
// <copyright file="UpdateLeaveDeductionCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.8.11">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Report
{
    using System;
    using Fanex.Data.Repository;

    public class UpdateLeaveDeductionCriteria : CriteriaBase
    {
        public int LeaveDeductionId { get; set; }

        public DateTime DeductDate { get; set; }

        public bool IsAnnual { get; set; }

        public decimal TotalDays { get; set; }

        public string Description { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "LeaveDeduction_Update";
        }

        public override bool IsValid()
        {
            return LeaveDeductionId > 0 && TotalDays > 0;
        }
    }
}
