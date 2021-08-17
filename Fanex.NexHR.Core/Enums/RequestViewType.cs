namespace Fanex.NexHR.Core.Enums
{
    using System.ComponentModel;

    public enum RequestViewType
    {
        [Description("LeaderView")]
        LeaderView = 1,

        [Description("HRView")]
        HRView = 2,

        [Description("StaffView")]
        StaffView = 5
    }
}