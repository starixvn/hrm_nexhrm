// --------------------------------------------------------------------------
// <copyright file="ShowOpenAndCloseAnnualBalance.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2014.12.29">Created</change>
// </history>

namespace Fanex.NexHR.Models.AnnualLeave
{
    using System;

    /// <summary>
    /// Condition Show Open And Close Annual Balance.
    /// </summary>
    public class ShowOpenAndCloseAnnualBalance
    {
        private const byte MaxMonth = 12;

        private const byte MaxDay = 31;

        public static DateTime FromDate => DateTime.Now.Month > 1 ? new DateTime(DateTime.Now.Year, MaxMonth, 1) : new DateTime(DateTime.Now.Year - 1, MaxMonth, 1);

        public static DateTime ToDate => DateTime.Now.Month > 1 ? new DateTime(DateTime.Now.Year + 1, 1, MaxDay) : new DateTime(DateTime.Now.Year, 1, MaxDay);

        public bool IsOpenedBalance { get; set; }

        public bool IsClosedBalance { get; set; }

        public bool IsShowOpenBalance => !IsOpenedBalance && DateTime.Now.Date >= FromDate && DateTime.Now.Date <= ToDate;

        public bool IsShowCloseBalance => !IsClosedBalance && DateTime.Now.Date >= FromDate && DateTime.Now.Date <= ToDate;
    }
}