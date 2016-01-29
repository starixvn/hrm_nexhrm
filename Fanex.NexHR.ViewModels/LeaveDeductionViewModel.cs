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
    using System.ComponentModel.DataAnnotations;
    using Fanex.NexHR.Resources;

    /// <summary>
    /// Leave Deduction ViewModel.
    /// </summary>
    public class LeaveDeductionViewModel
    {
        public int LeaveDeductionId { get; set; }

        public int EmpId { get; set; }

        [Display(Name = "DeductDate", ResourceType = typeof(Labels))]
        public DateTime DeductDate { get; set; }

        [Display(Name = "IsAnnual", ResourceType = typeof(Labels))]
        public bool IsAnnual { get; set; }

        [Display(Name = "TotalDays", ResourceType = typeof(Labels))]
        public decimal TotalDays { get; set; }

        public string Description { get; set; }
    }
}
