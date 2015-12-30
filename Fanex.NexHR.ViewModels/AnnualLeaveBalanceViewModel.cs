// --------------------------------------------------------------------------
// <copyright file="AnnualLeaveBalanceViewModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class AnnualLeaveBalanceViewModel
    {
        public AnnualLeaveBalanceViewModel()
        {
            AnnualLeaveTransactions = new List<AnnualLeaveBalanceModel>();
        }

        public int EmployeeId { get; set; }

        public int Year { get; set; }

        [Display(Name = "Include employee's request")]
        public bool ExcludedRequestTrans { get; set; }

        public IList<AnnualLeaveBalanceModel> AnnualLeaveTransactions { get; set; }
    }
}