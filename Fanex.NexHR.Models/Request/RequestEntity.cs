namespace Fanex.NexHR.Models.Request
{
    using System;
    using Fanex.NexHR.Core;

    public class RequestEntity : EntityBase<long>
    {
        private DateTime serverCreatedDate;

        private DateTime serverModifiedDate;

        private DateTime serverRequestedDate;

        private DateTime serverFromDate;

        private DateTime serverToDate;

        public Employee Requester { get; set; }

        public DateTime RequestedDate
        {
            get => serverRequestedDate;
            set => serverRequestedDate = value.ToClientTimeZone();
        }

        public RequestTypeModel RequestType { get; set; }

        public RequestStatus Status { get; set; }

        public Employee CreatedBy { get; set; }

        public DateTime CreatedDate
        {
            get => serverCreatedDate;
            set => serverCreatedDate = value.ToClientTimeZone();
        }

        public Employee ModifiedBy { get; set; }

        public DateTime ModifiedDate
        {
            get => serverModifiedDate;
            set => serverModifiedDate = value.ToClientTimeZone();
        }

        public bool IsPlanned { get; set; }

        public string Remark { get; set; }

        public decimal TotalDaysRequestInMonth { get; set; }

        public bool IsAllowUpdateStatus { get; set; }

        // Leave
        public DateTime FromDate
        {
            get => serverFromDate;
            set => serverFromDate = value.ToClientTimeZone();
        }

        public DateTime ToDate
        {
            get => serverToDate;
            set => serverToDate = value.ToClientTimeZone();
        }

        public decimal TotalDays { get; set; }

        public LeaveType LeaveType { get; set; }

        // ChangeShift
        public Shift FromShift { get; set; }

        public Shift ToShift { get; set; }

        // LateIn-EarlyOut
        public int TotalMins { get; set; }

        public bool IsCrossDate { get; set; }
    }
}
