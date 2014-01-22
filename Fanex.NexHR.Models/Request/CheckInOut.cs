// --------------------------------------------------------------------------
// <copyright file="CheckInOut.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Anders.Le" date="2012.5.18">Created</change>
// </history>

namespace Fanex.NexHR.Models.Request
{
    /// <summary>
    /// Check in/out.
    /// </summary>
    public class CheckInOut : RequestEntity
    {
        public Shift Shift { get; set; }
    }
}
