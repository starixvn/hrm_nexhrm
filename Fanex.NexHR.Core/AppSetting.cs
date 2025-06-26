namespace Fanex.NexHR.Core
{
    public class AppSetting
    {
        public string LoginView { get; set; } = "NexcelLogin";
        public int LoginType { get; set; } = 1;
        public bool IsShowCloseBalance { get; set; }
        public string ProjectManagerTeams { get; set; }
        public string IgnoreTeams { get; set; }
        public string DomainName { get; set; }
        public string IgnoreTeamWorkload { get; set; }
        public string IgnoreTeamLogTime { get; set; }
        public string IgnoreJobTitleLogTime { get; set; }
        public string IgnoreEmployeeLogTime { get; set; }
        public string IgnoreShowLeaveRequest { get; set; }
        public string RedmineBaseUrl { get; set; }
        public string RedmineDisplayUrl { get; set; }
        public string RedmineSecret { get; set; }
        public string ScheduledMessageUserGuide { get; set; }
        public string HRMName { get; set; }
        public string BaseUrl { get; set; }
        public string FBUrl { get; set; }
        public int FBCompanyId { get; set; }
        public string RijndaelKey { get; set; }
        public string RijndaelIV { get; set; }
        public bool EnableEncrypt { get; set; }
        public string NLogConfigFilePath { get; set; } = "NLog.config";
    }
}