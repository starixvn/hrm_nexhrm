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

    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // common
            bundles.Add(new ScriptBundle("~/bundles/sitejs").Include(
                        "~/Scripts/Libs/jquery-{version}.js",
                        "~/Scripts/Libs/jquery-ui-{version}.js",
                        "~/Scripts/Libs/jquery.validate*",
                        "~/Scripts/Libs/materialize.js",
                        "~/Scripts/Libs/jquery.tipsy.js",
                        "~/Scripts/Libs/jquery.cookie.js",
                        "~/Scripts/Libs/chart.js",
                        "~/Scripts/Libs/common.js",
                        "~/Scripts/Views/language.js"));

            bundles.AddAndInclude(
                "~/Content/sitecss",
                "~/Content/Materialize/materialize.css",
                "~/Content/Common/components/materialdesignicons.css",
                "~/Content/site.css",
                "~/Content/themes/base/jquery-ui.css");

            bundles.AddAndInclude(
                "~/Content/fullcalendarcss",
                "~/Content/Common/fullcalendar.css");

            bundles.AddAndInclude(
                "~/Content/contextmenucss",
                "~/Content/ContextMenu/contextMenu.css");

            bundles.AddAndInclude(
                "~/Content/myrequestcss",
                "~/Content/Views/myrequest.css");

            // dashboard
            bundles.Add(new ScriptBundle("~/bundles/dashboardjs").Include(
                "~/Scripts/Views/employee.dashboard.js"));

            bundles.AddAndInclude(
                "~/Content/dashboardcss",
                "~/Content/Views/mydashboard.css");

            // leave request
            bundles.Add(new ScriptBundle("~/bundles/leavejs").Include(
                "~/Scripts/Libs/jquery.contextMenu.js",
                "~/Scripts/Views/leave.request.js",
                "~/Scripts/Views/checkinout.byemployee.js",
                "~/Scripts/Views/search.request.js"));

            // check in out
            bundles.Add(new ScriptBundle("~/bundles/empcheckinoutjs").Include(
                "~/Scripts/Libs/jquery.contextMenu.js",
                "~/Scripts/Views/checkinout.request.js",
                "~/Scripts/Views/checkinout.byemployee.js",
                "~/Scripts/Views/search.request.js"));

            // my working schedule
            bundles.Add(new ScriptBundle("~/bundles/myworkingschedulejs").Include(
                "~/Scripts/Libs/moment.js",
                "~/Scripts/Libs/fullcalendar.js",
                "~/Scripts/Views/myworkingschedule.js"));
        }
    }
}