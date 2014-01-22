// --------------------------------------------------------------------------
// <copyright file="RolesEnum.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>

namespace Fanex.NexHR.Core.Enums
{
    public enum RolesEnum : short
    {
        [StringValue("Member")]
        Member = 0,

        [StringValue("AssistantLeader")]
        AssistantLeader = 1,

        [StringValue("Leader")]
        Leader = 2,

        [StringValue("HR")]
        HR = 3,

        [StringValue("SA")]
        SA = 4
    }
}