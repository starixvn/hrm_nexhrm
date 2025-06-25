namespace Fanex.NexHR.Core;

using System;
using Fanex.NexHR.Core;

public static class DateTimeExtensions
{
    private static readonly TimeSpan ServerTimeZone = TimeSpan.FromHours(-4);
    private static readonly TimeSpan ClientTimeZone = TimeSpan.FromHours(7);
    public static DateTime ToServerTimeZone(this DateTime time)
    {
        var offsetTime = new DateTimeOffset(time, ClientTimeZone);
        return offsetTime.ToOffset(ServerTimeZone).DateTime;
    }

    public static DateTime ToClientTimeZone(this DateTime time)
    {
        var offsetTime = new DateTimeOffset(time, ServerTimeZone);
        return offsetTime.ToOffset(ClientTimeZone).DateTime;
    }
}