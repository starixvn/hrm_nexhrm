// --------------------------------------------------------------------------
// <copyright file="Commands.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Core.Enums
{
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:EnumerationItemsMustBeDocumented", Justification = "Reviewed.")]
    public enum Commands
    {
        [StringValue("Approve")]
        Approve = 1,

        [StringValue("Disapprove")]
        Disapprove = 2,

        [StringValue("Reject")]
        Reject = 3
    }
}
