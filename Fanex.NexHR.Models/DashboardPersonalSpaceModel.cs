// --------------------------------------------------------------------------
// <copyright file="DashboardPersonalSpaceModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.8.18">Created</change>
// </history>

namespace Fanex.NexHR.Models
{
    using System;

    public class DashboardPersonalSpaceModel
    {
        public Employee Employee { get; set; }

        public DateTime WorkDate { get; set; }

        public bool IsCurrent { get; set; }

        public short TypeId { get; set; }

        public float LeaveDay { get; set; }
    }
}
