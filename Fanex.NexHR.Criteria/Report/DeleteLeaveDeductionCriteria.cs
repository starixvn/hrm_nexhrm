// --------------------------------------------------------------------------
// <copyright file="DeleteLeaveDeductionCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.8.11">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Report
{
    using Fanex.Data.Repository;

    public class DeleteLeaveDeductionCriteria : CriteriaBase
    {
        public int LeaveDeductionId { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "LeaveDeduction_Delete";
        }

        public override bool IsValid()
        {
            return LeaveDeductionId > 0;
        }
    }
}
