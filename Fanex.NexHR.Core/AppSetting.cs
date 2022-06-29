namespace Fanex.NexHR.Core
{
    using System.Configuration;

    public static class AppSetting
    {
        public static bool IsShowCloseBalance => bool.Parse(ConfigurationManager.AppSettings["IsCloseBalance"]);

        public static string ProjectManagerTeams => ConfigurationManager.AppSettings["ProjectManagerTeams"];

        public static string IgnoreTeams => ConfigurationManager.AppSettings["IgnoreTeams"];

        public static string DomainName => ConfigurationManager.AppSettings["domain"];

        public static string IgnoreTeamWorkload => ConfigurationManager.AppSettings["IgnoreTeamWorkload"];

        public static string Redmine_API_Address => ConfigurationManager.AppSettings["Redmine_API_Address"];

        public static string RedmineConnectionString => ConfigurationManager.AppSettings["Redmine_ConnectionString"];

        public static string ScheduledMessageUserGuide => ConfigurationManager.AppSettings["ScheduledMessage_UserGuide"];

        public static string HRMName => ConfigurationManager.AppSettings["HRMName"];

        public static string LDAP => ConfigurationManager.AppSettings["LDAP"];

        public static string BaseUrl => ConfigurationManager.AppSettings["BaseUrl"];
    }
}