namespace Fanex.NexHR.Models.AnnualLeave
{
    using System;
    using Fanex.NexHR.Core;

    public class ShowOpenAndCloseAnnualBalance
    {
        private const byte MaxMonth = 12;

        private const byte MaxDay = 31;

        public static DateTime FromDate => DateTimeOffset.Now.DateTime.ToClientTimeZone().Month > 1
            ? new DateTimeOffset(DateTimeOffset.Now.DateTime.ToClientTimeZone().Year, MaxMonth, 1, 0, 0, 0, TimeSpan.Zero).DateTime
            : new DateTimeOffset(DateTimeOffset.Now.DateTime.ToClientTimeZone().Year - 1, MaxMonth, 1, 0, 0, 0, TimeSpan.Zero).DateTime;

        public static DateTime ToDate => DateTimeOffset.Now.DateTime.ToClientTimeZone().Month > 1 
            ? new DateTimeOffset(DateTimeOffset.Now.DateTime.ToClientTimeZone().Year + 1, 1, MaxDay, 0, 0, 0, TimeSpan.Zero).DateTime
            : new DateTimeOffset(DateTimeOffset.Now.DateTime.ToClientTimeZone().Year, 1, MaxDay, 0, 0, 0, TimeSpan.Zero).DateTime;

        public bool IsOpenedBalance { get; set; }

        public bool IsClosedBalance { get; set; }

        public bool IsShowOpenBalance => !IsOpenedBalance && DateTimeOffset.Now.DateTime.ToClientTimeZone().Date >= FromDate && DateTimeOffset.Now.DateTime.ToClientTimeZone().Date <= ToDate;

        public bool IsShowCloseBalance => !IsClosedBalance && DateTimeOffset.Now.DateTime.ToClientTimeZone().Date >= FromDate && DateTimeOffset.Now.DateTime.ToClientTimeZone().Date <= ToDate;
    }
}