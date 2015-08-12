// --------------------------------------------------------------------------
// <copyright file="InsertLeaveDeductionCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.8.7">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Report
{
    using System;
    using Fanex.Data.Repository;

    public class InsertLeaveDeductionCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public DateTime DeductDate { get; set; }

        public bool IsAnnual { get; set; }

        public decimal TotalDays { get; set; }

        public string Description { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey()
        {
            return "LeaveDeduction_Insert";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0 && TotalDays > 0;
        }
    }
}
