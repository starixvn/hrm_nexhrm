// --------------------------------------------------------------------------
// <copyright file="Command.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Core.Enums
{
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Reviewed. Suppression is OK here.")]
    public enum Command
    {
        /// <summary>
        /// Approve command.
        /// </summary>
        [StringValue("Approve")]
        Approve = 1,

        /// <summary>
        /// Disapprove command.
        /// </summary>
        [StringValue("Disapprove")]
        Disapprove = 2,

        /// <summary>
        /// Reject command.
        /// </summary>
        [StringValue("Reject")]
        Reject = 3
    }
}
