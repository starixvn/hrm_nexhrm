// --------------------------------------------------------------------------
// <copyright file="LeaveDeductionDetail.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.8.10">Created</change>
// </history>

namespace Fanex.NexHR.Models.Report
{
    using System;

    public class LeaveDeductionDetail
    {
        public int LeaveDeductionId { get; set; }

        public Employee Employee { get; set; }

        public DateTime DeductDate { get; set; }

        public bool IsAnnual { get; set; }

        public float LeaveDays { get; set; }

        public string Description { get; set; }

        public Employee ModifiedBy { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
