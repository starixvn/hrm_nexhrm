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
    public enum RequestTypes : short
    {
        /// <summary>
        /// Check in request.
        /// </summary>
        [StringValue("Check In")]
        CheckIn = 1,

        /// <summary>
        /// Check out request.
        /// </summary>
        [StringValue("Check Out")]
        CheckOut = 2,

        /// <summary>
        /// Late in request.
        /// </summary>
        [StringValue("Late In")]
        LateIn = 3,

        /// <summary>
        /// Early out request.
        /// </summary>
        [StringValue("Early Out")]
        EarlyOut = 4,

        /// <summary>
        /// Change shift request.
        /// </summary>
        [StringValue("Change Shift")]
        ChangeShift = 5,

        /// <summary>
        /// Leave request.
        /// </summary>
        [StringValue("Leave")]
        Leave = 6
    }
}
