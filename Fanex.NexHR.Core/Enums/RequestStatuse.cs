// --------------------------------------------------------------------------
// <copyright file="RequestStatuse.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>

namespace Fanex.NexHR.Core.Enums
{
    using System.Diagnostics.CodeAnalysis;
    using Fanex.NexHR.Core.Attributes;

    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Reviewed. Suppression is OK here.")]
    public enum RequestStatuse
    {
        /// <summary>
        /// Pending status.
        /// </summary>
        [StringValue("Pending")]
        Pending = 1,

        /// <summary>
        /// Approved status.
        /// </summary>
        [StringValue("Approved")]
        Approve = 2,

        /// <summary>
        /// Rejected status.
        /// </summary>
        [StringValue("Rejected")]
        Reject = 3
    }
}