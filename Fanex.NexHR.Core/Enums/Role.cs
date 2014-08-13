// --------------------------------------------------------------------------
// <copyright file="Role.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>

namespace Fanex.NexHR.Core.Enums
{
    using Fanex.NexHR.Core.Attributes;

    public enum Role : int
    {
        /// <summary>
        /// Member role.
        /// </summary>
        [StringValue("Member")]
        Member = 0,

        /// <summary>
        /// Assistant leader role.
        /// </summary>
        [StringValue("AssistantLeader")]
        AssistantLeader = 1,

        /// <summary>
        /// Leader role.
        /// </summary>
        [StringValue("Leader")]
        Leader = 2,

        /// <summary>
        /// HR department role.
        /// </summary>
        [StringValue("HR")]
        HR = 3,

        /// <summary>
        /// System admin role.
        /// </summary>
        [StringValue("SA")]
        SA = 4
    }
}