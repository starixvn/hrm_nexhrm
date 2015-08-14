// --------------------------------------------------------------------------
// <copyright file="RequestsMonitoring.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Models
{
    /// <summary>
    /// Requests monitoring.
    /// </summary>
    public class RequestsMonitoring
    {
        public Employee Requester { get; set; }

        public int TotalLeaveTimes { get; set; }

        public float TotalAnnualLeave { get; set; }

        public float TotalUnpaidLeave { get; set; }

        public float TotalOtherLeave { get; set; }

        public int TotalCheckInOutTimes { get; set; }

        public int TotalMissingCheckInOutTimes { get; set; }

        public int LateInEarlyOutTimes { get; set; }

        public int TotalAbsent { get; set; }
    }
}
