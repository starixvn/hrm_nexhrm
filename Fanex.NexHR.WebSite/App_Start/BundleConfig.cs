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
            #region Common

            bundles.Add(new ScriptBundle("~/bundles/sitejs").Include(
                        "~/Scripts/Libs/jquery-{version}.js",
                        "~/Scripts/Libs/jquery-ui-{version}.js",
                        "~/Scripts/Libs/jquery.validate*",
                        "~/Scripts/Libs/jquery-dateFormat.min.js",
                        "~/Scripts/Libs/materialize.js",
                        "~/Scripts/Libs/jquery.tipsy.js",
                        "~/Scripts/Libs/jquery.tmpl.min.js",
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

            // full calendar
            bundles.Add(new ScriptBundle("~/bundles/fullcalendarjs").Include(
                "~/Scripts/Libs/moment.js",
                "~/Scripts/Libs/fullcalendar.js"));

            bundles.AddAndInclude(
                "~/Content/fullcalendarcss",
                "~/Content/Common/fullcalendar.css");

            // date js
            bundles.Add(new ScriptBundle("~/bundles/datejs").Include(
                "~/Scripts/Libs/date.js"));

            // context menu
            bundles.Add(new ScriptBundle("~/bundles/contextmenujs").Include(
                "~/Scripts/Libs/jquery.contextMenu.js"));

            bundles.AddAndInclude(
                "~/Content/contextmenucss",
                "~/Content/ContextMenu/contextMenu.css");

            // month picker
            bundles.AddAndInclude(
                "~/Content/monthpickercss",
                "~/Content/Common/monPickr-0.0.3.css");

            bundles.Add(new ScriptBundle("~/bundles/monthpickerjs").Include(
                "~/Scripts/Libs/monPickr-0.0.3.js"));

            bundles.AddAndInclude(
                "~/Content/myrequestcss",
                "~/Content/Views/myrequest.css");

            #endregion Common

            // dashboard
            bundles.Add(new ScriptBundle("~/bundles/dashboardjs").Include(
                "~/Scripts/Views/employee.dashboard.js"));

            bundles.AddAndInclude(
                "~/Content/dashboardcss",
                "~/Content/Views/mydashboard.css");

            // leave request
            bundles.Add(new ScriptBundle("~/bundles/leavejs").Include(
                "~/Scripts/Views/leave.request.js",
                "~/Scripts/Views/checkinout.byemployee.js",
                "~/Scripts/Views/search.request.js"));

            // check in out
            bundles.Add(new ScriptBundle("~/bundles/empcheckinoutjs").Include(
                "~/Scripts/Views/checkinout.request.js",
                "~/Scripts/Views/checkinout.byemployee.js",
                "~/Scripts/Views/search.request.js"));

            // my timesheet
            bundles.Add(new ScriptBundle("~/bundles/mytimesheetjs").Include(
                "~/Scripts/Views/report.timesheet.js"));

            bundles.AddAndInclude(
                "~/Content/mytimesheetcss",
                "~/Content/Views/report.timesheet.css");

            // time record
            bundles.Add(new ScriptBundle("~/bundles/timerecordjs").Include(
                "~/Scripts/Views/report.timerecord.js"));

            bundles.AddAndInclude(
                "~/Content/timerecordcss");

            // my calendar
            bundles.Add(new ScriptBundle("~/bundles/mycalendarjs").Include(
                "~/Scripts/Views/mycalendar.js"));

            bundles.AddAndInclude(
                "~/Content/mycalendarcss",
                "~/Content/Views/mycalendar.css");
        }
    }
}