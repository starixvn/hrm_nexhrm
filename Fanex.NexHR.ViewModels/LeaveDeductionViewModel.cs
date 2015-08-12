// --------------------------------------------------------------------------
// <copyright file="LeaveDeductionViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.8.5">Created</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System;

    /// <summary>
    /// Leave Deduction ViewModel.
    /// </summary>
    public class LeaveDeductionViewModel
    {
        public int LeaveDeductionId { get; set; }

        public int EmpId { get; set; }

        public DateTime DeductDate { get; set; }

        public bool IsAnnual { get; set; }

        public decimal TotalDays { get; set; }

        public string Description { get; set; }
    }
}
