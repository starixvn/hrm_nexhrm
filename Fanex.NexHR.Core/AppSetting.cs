// --------------------------------------------------------------------------
// <copyright file="AppSetting.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Core
{
    using System.Configuration;
    using Fanex.Common;

    public static class AppSetting
    {
        public static bool IsShowCloseBalance
        {
            get 
            {
                return Converter.ToBool(ConfigurationManager.AppSettings["IsCloseBalance"].ToString());
            }
        }

        public static string ProjectManagerTeams
        {
            get 
            {
                return ConfigurationManager.AppSettings["ProjectManagerTeams"].ToString(); 
            }
        }

        public static string IgnoreTeams
        {
            get
            {
                return ConfigurationManager.AppSettings["IgnoreTeams"].ToString();
            }
        }

        public static string DomainName
        {
            get
            {
                return ConfigurationManager.AppSettings["domain"].ToString();
            }
        }
    }
}
