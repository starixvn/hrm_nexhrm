// --------------------------------------------------------------------------
// <copyright file="GeneralRequestType.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.8.14">Created</change>
// </history>

namespace Fanex.NexHR.Core.Enums
{
    using Fanex.NexHR.Core.Attributes;

    /// <summary>
    /// ENUM General Leave Type.
    /// </summary>
    public enum GeneralRequestType : int
    {
        /// <summary>
        /// Leave request.
        /// </summary>
        [StringValue("Leave")]
        Leave = 0,

        /// <summary>
        /// Late in early out request.
        /// </summary>
        [StringValue("LateInEarlyOut")]
        LateInEarlyOut = 1,

        /// <summary>
        /// Check in out request.
        /// </summary>
        [StringValue("CheckInOut")]
        CheckInOut = 2,
    }
}
