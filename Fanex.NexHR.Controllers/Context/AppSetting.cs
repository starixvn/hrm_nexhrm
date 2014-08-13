// --------------------------------------------------------------------------
// <copyright file="AppSetting.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.15">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Controllers.Context
{
    using System.Configuration;
    using Fanex.Common;

    /// <summary>
    /// Application setting.
    /// </summary>
    public static class AppSetting
    {
        private static bool _isShowCloseBalance = true;

        public static bool IsShowCloseBalance
        {
            get { return InitializeResourceString(); }
        }

        private static bool InitializeResourceString()
        {
            _isShowCloseBalance = Converter.ToBool(ConfigurationManager.AppSettings["IsCloseBalance"].ToString());
            return _isShowCloseBalance;
        }
    }
}
