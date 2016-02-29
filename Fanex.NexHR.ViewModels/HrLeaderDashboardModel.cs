namespace Fanex.NexHR.ViewModels
{
    using System;

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
