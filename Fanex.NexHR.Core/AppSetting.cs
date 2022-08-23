namespace Fanex.NexHR.Core
{
    public class AppSetting
    {
        public string LoginView { get; set; } = "NexcelLogin";
        public bool IsShowCloseBalance { get; set; }
        public string ProjectManagerTeams { get; set; }
        public string IgnoreTeams { get; set; }
        public string DomainName { get; set; }
        public string IgnoreTeamWorkload { get; set; }
        public string RedmineBaseUrl { get; set; }
        public string RedmineSecret { get; set; }
        public string ScheduledMessageUserGuide { get; set; }
        public string HRMName { get; set; }
        public string LdapUrl { get; set; }
        public string BaseUrl { get; set; }
        public string AppMailAddress { get; set; }
        public string AppMailId { get; set; }
    }
}