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
    using System.Diagnostics.CodeAnalysis;
    using Fanex.Common;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1309:FieldNamesMustNotBeginWithUnderscore", Justification = "Reviewed.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed.")]
    public class AppSetting
    {
        private static bool _isShowCloseBalance = true;

        static AppSetting()
        {
            try
            {
                _isShowCloseBalance = Converter.ToBool(ConfigurationManager.AppSettings["IsCloseBalance"].ToString());
            }
            catch
            {
            }
        }

        public static bool IsShowCloseBalance
        {
            get { return _isShowCloseBalance; }
        }
    }
}
