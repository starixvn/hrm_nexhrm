// --------------------------------------------------------------------------
// <copyright file="BundleConfig.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.11.6">Created</change>
// </history>

namespace Fanex.NexHR.WebSite
{
    using System.Web.Optimization;

    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // common
            bundles.Add(new ScriptBundle("~/bundles/sitejs").Include(
                        "~/Scripts/Libs/jquery-{version}.js",
                        "~/Scripts/Libs/jquery-ui-{version}.js",
                        "~/Scripts/Libs/jquery.validate*",
                        "~/Scripts/Libs/jquery.tmpl.js",
                        "~/Scripts/Libs/jquery.tipsy.js",
                        "~/Scripts/Libs/jquery.cookie.js",
                        "~/Scripts/Libs/common.js",
                        "~/Scripts/Views/language.js"));

            bundles.Add(new StyleBundle("~/Content/sitecss").Include("~/Content/site.css"));

            // dashboard
            bundles.Add(new ScriptBundle("~/bundles/dashboardjs").Include("~/Scripts/Views/employee.dashboard.js"));

            bundles.Add(new StyleBundle("~/Content/dashboardcss").Include("~/Content/mydashboard.css"));
        }
    }
}