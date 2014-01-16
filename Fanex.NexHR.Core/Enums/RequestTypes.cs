// --------------------------------------------------------------------------
// <copyright file="RequestTypes.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>

namespace Fanex.NexHR.Core.Enums
{
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:EnumerationItemsMustBeDocumented", Justification = "Reviewed.")]
    public enum RequestTypes : short
    {
        [StringValue("Check In")]
        CheckIn = 1,
        [StringValue("Check Out")]
        CheckOut = 2,
        [StringValue("Late In")]
        LateIn = 3,
        [StringValue("Early Out")]
        EarlyOut = 4,
        [StringValue("Change Shift")]
        ChangeShift = 5,
        [StringValue("Leave")]
        Leave = 6
    }
}
