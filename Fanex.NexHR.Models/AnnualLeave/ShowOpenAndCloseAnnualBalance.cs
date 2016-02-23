// --------------------------------------------------------------------------
// <copyright file="ShowOpenAndCloseAnnualBalance.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
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
        public DateTime FromDate 
        {
            get 
            {
                if (DateTime.Now.Month > 1)
                {
                    return new DateTime(DateTime.Now.Year, 12, 1);
                }
                else
                {
                    return new DateTime(DateTime.Now.Year - 1, 12, 1);
                }
            }
        }

        public DateTime ToDate
        {
            get
            {
                if (DateTime.Now.Month > 1)
                {
                    return new DateTime(DateTime.Now.Year + 1, 1, 31);
                }
                else
                {
                    return new DateTime(DateTime.Now.Year, 1, 31);
                }
            }
        }

        public bool IsOpenedBalance { get; set; }

        public bool IsClosedBalance { get; set; }

        public bool IsShowOpenBalance
        {
            get
            {
                return !IsOpenedBalance && DateTime.Now.Date >= FromDate && DateTime.Now.Date <= ToDate;
            }
        }

        public bool IsShowCloseBalance
        {
            get
            {
                return !IsClosedBalance && DateTime.Now.Date >= FromDate && DateTime.Now.Date <= ToDate;
            }
        }
    }
}
