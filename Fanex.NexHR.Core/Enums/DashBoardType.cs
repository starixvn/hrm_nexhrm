// --------------------------------------------------------------------------
// <copyright file="DashBoardType.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.8.19">Created</change>
// </history>

namespace Fanex.NexHR.Core.Enums
{
    using Attributes;

    public enum DashBoardType
    {
        /// <summary>
        /// None value.
        /// </summary>
        [StringValue("None")]
        None = 0,

        /// <summary>
        /// Leave request.
        /// </summary>
        [StringValue("Leave")]
        Leave = 1,

        /// <summary>
        /// Check in out request.
        /// </summary>
        [StringValue("CheckInOut")]
        CheckInOut = 2,

        /// <summary>
        /// Missing Check in out request.
        /// </summary>
        [StringValue("MissingCheckInOut")]
        MissingCheckInOut = 3,

        /// <summary>
        /// Late In, Early Out.
        /// </summary>
        [StringValue("LateInEarlyOut")]
        LateInEarlyOut = 4,

        /// <summary>
        /// Absent.
        /// </summary>
        [StringValue("Absent")]
        Absent = 5
    }
}