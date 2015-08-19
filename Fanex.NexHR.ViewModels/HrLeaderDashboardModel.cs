// --------------------------------------------------------------------------
// <copyright file="HRLeaderDashboardModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.ViewModels
{
    using System;

    /// <summary>
    /// HR leader dashboard model.
    /// </summary>
    public class HRLeaderDashboardModel
    {
        public int LeavePending { get; set; }

        public int CheckInOutPending { get; set; }

        public DateTime FromDateLeave { get; set; }

        public DateTime ToDateLeave { get; set; }

        public DateTime FromDateCheckInOut { get; set; }

        public DateTime ToDateCheckInOut { get; set; }

        public int LeaveApprovedCurrentMonth { get; set; }

        public int LeaveApprovedLastMonth { get; set; }

        public int LeaveRejectedCurrentMonth { get; set; }

        public int LeaveRejectedLastMonth { get; set; }

        public int CheckInOutApprovedCurrentMonth { get; set; }

        public int CheckInOutApprovedLastMonth { get; set; }

        public int CheckInOutRejectedCurrentMonth { get; set; }

        public int CheckInOutRejectedLastMonth { get; set; }
    }
}
