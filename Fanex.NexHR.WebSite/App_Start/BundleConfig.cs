namespace Fanex.NexHR.WebSite
{
    using System.Web.Optimization;

    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            BuildBundleForCommon(bundles);

            // full calendar
            bundles.Add(new ScriptBundle("~/bundles/fullcalendarjs").Include(
                "~/Scripts/Libs/moment.js",
                "~/Scripts/Libs/fullcalendar.js"));

            bundles.AddAndInclude(
                "~/Content/fullcalendarcss",
                "~/Content/Common/fullcalendar.css");

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

            // member requests
            bundles.Add(new ScriptBundle("~/bundles/managerequestsjs").Include(
                "~/Scripts/Views/mgr.requests.js",
                "~/Scripts/Views/_employee.search.in.team.js"));

            // requests statistic
            bundles.Add(new ScriptBundle("~/bundles/requeststatisticjs").Include(
                "~/Scripts/Views/leader.request.monitoring.js"));

            // annual leave balance
            bundles.Add(new ScriptBundle("~/bundles/annualleavebalancejs").Include(
                "~/Scripts/Views/annualleavebalance.js",
                "~/Scripts/Views/_employee.search.in.company.js"));

            BuildBundleForExtension(bundles);

            BuildBundleForMemo(bundles);

            // user management
            bundles.Add(new ScriptBundle("~/bundles/usermanagementjs").Include(
                "~/Scripts/Views/user.management.js"));

            // team management
            bundles.Add(new ScriptBundle("~/bundles/teammanagementjs").Include(
                "~/Scripts/Views/team.management.js"));

            BuildBundleForToeic(bundles);
        }

        private static void BuildBundleForExtension(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/addextensionnojs").Include(
                "~/Scripts/Views/addextensionno.js"));
            bundles.Add(new ScriptBundle("~/bundles/extensionnojs").Include(
                "~/Scripts/Views/extensionno.js"));
        }

        private static void BuildBundleForMemo(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/memoeditjs").Include(
                "~/Scripts/Views/memo.js",
                "~/Scripts/tiny_mce/tiny_mce_src.js",
                "~/Scripts/Views/memo.edit.js"));
            bundles.AddAndInclude(
                "~/Content/annualleavebalancecss",
                "~/Content/Views/annualleavebalance.css");
        }

        private static void BuildBundleForToeic(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/toeicjs").Include(
                "~/Scripts/Views/Employee/employee.toeic.js"));
        }

        private static void BuildBundleForCommon(BundleCollection bundles)
        {
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
                         "~/Scripts/Libs/date.js",
                         "~/Scripts/Libs/common.js",
                         "~/Scripts/Views/language.js"));

            bundles.AddAndInclude(
                "~/Content/sitecss",
                "~/Content/Materialize/materialize.css",
                "~/Content/Common/components/materialdesignicons.css",
                "~/Content/site.css",
                "~/Content/themes/base/jquery-ui.css");
        }
    }
}