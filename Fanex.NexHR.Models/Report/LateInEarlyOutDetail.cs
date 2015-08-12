// --------------------------------------------------------------------------
// <copyright file="LateInEarlyOutDetail.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2015.8.10">Created</change>
// </history>

namespace Fanex.NexHR.Models.Report
{
    using System;

    public class LateInEarlyOutDetail
    {
        public Employee Employee { get; set; }

        public DateTime WorkDate { get; set; }

        public int Type { get; set; }

        public int LeaveDeductionId { get; set; }

        public int Minutes { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
