// --------------------------------------------------------------------------
// <copyright file="RequestTypeFilter.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.01.14">Created</change>
// </history>

namespace Fanex.NexHR.Core.Enums
{
    using System.ComponentModel;

    public enum RequestTypeFilter : int
    {
        [Description("All")]
        All = 0,

        [Description("Check In")]
        CheckIn = 1,

        [Description("Check Out")]
        CheckOut = 2,

        [Description("Late In")]
        LateIn = 3,

        [Description("Early Out")]
        EarlyOut = 4,

        [Description("Not Working")]
        ChangeShift = 5
    }
}