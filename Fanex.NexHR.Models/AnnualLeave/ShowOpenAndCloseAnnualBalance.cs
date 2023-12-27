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

        public static DateTime FromDate => DateTimeOffset.Now.Month > 1
        ? new DateTimeOffset(DateTimeOffset.Now.Year, MaxMonth, 1, 0, 0, 0, TimeSpan.Zero).DateTime
        : new DateTimeOffset(DateTimeOffset.Now.Year - 1, MaxMonth, 1, 0, 0, 0, TimeSpan.Zero).DateTime;

        public static DateTime ToDate => DateTimeOffset.Now.Month > 1 
        ? new DateTimeOffset(DateTimeOffset.Now.Year + 1, 1, MaxDay, 0, 0, 0, TimeSpan.Zero).DateTime
        : new DateTimeOffset(DateTimeOffset.Now.Year, 1, MaxDay, 0, 0, 0, TimeSpan.Zero).DateTime;

        public bool IsOpenedBalance { get; set; }

        public bool IsClosedBalance { get; set; }

        public bool IsShowOpenBalance => !IsOpenedBalance && DateTimeOffset.Now.DateTime.Date >= FromDate && DateTimeOffset.Now.DateTime.Date <= ToDate;

        public bool IsShowCloseBalance => !IsClosedBalance && DateTimeOffset.Now.DateTime.Date >= FromDate && DateTimeOffset.Now.DateTime.Date <= ToDate;
    }
}