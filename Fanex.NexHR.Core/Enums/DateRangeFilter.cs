// --------------------------------------------------------------------------
// <copyright file="DateRangeFilter.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.01.14">Created</change>
// </history>

namespace Fanex.NexHR.Core
{
    using System.ComponentModel;

    public enum DateRangeFilter
    {
        [Description("Date Range")]
        DateRange = 0,

        [Description("Today")]
        ToDay = 1,

        [Description("Yesterday")]
        Yesterday = 2,

        [Description("Current Week")]
        CurrentWeek = 3,

        [Description("Last Week")]
        LastWeek = 4,

        [Description("Current Month")]
        CurrentMonth = 5,

        [Description("Last Month")]
        LastMonth = 6,

        [Description("Since Last Month")]
        SinceLastMonth = 7
    }
}