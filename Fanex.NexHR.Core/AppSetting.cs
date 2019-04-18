// --------------------------------------------------------------------------
// <copyright file="AppSetting.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Core
{
    using System.Configuration;
    using Common;

    public static class AppSetting
    {
        public static bool IsShowCloseBalance => Converter.ToBool(ConfigurationManager.AppSettings["IsCloseBalance"]);

        public static string ProjectManagerTeams => ConfigurationManager.AppSettings["ProjectManagerTeams"];

        public static string IgnoreTeams => ConfigurationManager.AppSettings["IgnoreTeams"];

        public static string DomainName => ConfigurationManager.AppSettings["domain"];

        public static string IgnoreTeamWorkload => ConfigurationManager.AppSettings["IgnoreTeamWorkload"];

        public static string Redmine_API_Address => ConfigurationManager.AppSettings["Redmine_API_Address"];

        public static string RedmineConnectionString => Cryptography.CfsDeCrypt(ConfigurationManager.AppSettings["Redmine_ConnectionString"]);

        public static string ScheduledMessageUserGuide => ConfigurationManager.AppSettings["ScheduledMessage_UserGuide"];
    }
}