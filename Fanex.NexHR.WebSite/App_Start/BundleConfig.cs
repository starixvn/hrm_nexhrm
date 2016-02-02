namespace Fanex.NexHR.WebSite
{
    using System.Web.Optimization;

    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            BuildBundleForCommon(bundles);

            BuildBundleForDashboard(bundles);

            BuildBundleForLeaveRequest(bundles);

            BuildBundleForCheckInOutRequest(bundles);

            BuildBundleForMyTimeSheetReport(bundles);

            BuildBundleForReportMyTimeRecord(bundles);

            BuildBundleForMyCalendar(bundles);

            BuildBundleForMemberRequests(bundles);

            BuildBundleForRequestsStatistic(bundles);

            BuildBundleForAnnualLeaveBalance(bundles);

            BuildBundleForTotalWorkingHours(bundles);

            BuildBundleForHolidays(bundles);

            BuildBundleForGeolocation(bundles);

            BuildBundleForExtension(bundles);

            BuildBundleForMemo(bundles);

            BuildBundleForUserManagement(bundles);

            BuildBundleForTeamManagement(bundles);

            BuildBundleForToeic(bundles);

            BuildBundleForJobTitle(bundles);

            BuildBundleForAttendanceReport(bundles);

            BuildBundleForEditEmployee(bundles);

            BuildBundleForLateInEarlyOut(bundles);

            BuildBundleForDefaultSchedule(bundles);

            BuildBundleForMonthSchedule(bundles);

            BuildBundleForReportTimeRecord(bundles);

            BuildBundleForLeaveSummary(bundles);

            BuildBundleForWorkingCanlendar(bundles);

            BuildBundleForStaffInfo(bundles);

            BuildBundleForStaffBirthday(bundles);

            BuildBundleForLeaveDeductionDetail(bundles);

            BuildBundlerForOvertime(bundles);

            BuildBundleForLateInEarlyOutDetail(bundles);

            BuildBundleForMyProfile(bundles);

            BuildBundleForSimplePagination(bundles);
        }

        private static void BuildBundlerForOvertime(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/createovertimejs").Include(
                         "~/Scripts/Views/Overtime/create.js"));

            bundles.Add(new ScriptBundle("~/bundles/summaryovertimejs").Include(
                         "~/Scripts/Views/Overtime/summary.js"));

            bundles.Add(new ScriptBundle("~/bundles/editovertimejs").Include(
                         "~/Scripts/Views/Overtime/edit.js"));
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
                         "~/Scripts/Libs/jsrender.min.js",
                         "~/Scripts/Libs/jquery.cookie.js",
                         "~/Scripts/Libs/chart.js",
                         "~/Scripts/Libs/date.js",
                         "~/Scripts/Libs/jquery.floatThead.min.js",
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

            // context menu
            bundles.Add(new ScriptBundle("~/bundles/contextmenujs").Include(
                "~/Scripts/Libs/jquery.contextMenu.min.js"));

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
        }

        private static void BuildBundleForTotalWorkingHours(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/totalworkinghourjs").Include(
                "~/Scripts/Views/_employee.search.in.company.js",
                "~/Scripts/Views/totalworkinghours.js"));
        }

        private static void BuildBundleForHolidays(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/holidays").Include(
                "~/Scripts/Views/holiday.js"));
            bundles.Add(new ScriptBundle("~/bundles/holidayedit").Include(
                "~/Scripts/Views/holiday.edit.js"));
        }

        private static void BuildBundleForGeolocation(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/geolocationjs").Include(
                "~/Scripts/Views/geolocationmap.js"));

            bundles.AddAndInclude(
                "~/Content/geolocationcss",
                "~/Content/Views/geolocation.css");
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
            bundles.Add(new ScriptBundle("~/bundles/memoindexjs").Include(
                "~/Scripts/Views/Memo/memo.index.js"));

            bundles.Add(new ScriptBundle("~/bundles/memoeditjs").Include(
                "~/Scripts/Views/Memo/memo.js",
                "~/Scripts/tiny_mce/tiny_mce_src.js",
                "~/Scripts/Views/Memo/memo.edit.js"));

            bundles.AddAndInclude(
                "~/Content/annualleavebalancecss",
                "~/Content/Views/annualleavebalance.css");
        }

        private static void BuildBundleForToeic(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/toeicjs").Include(
                "~/Scripts/Views/Employee/employee.toeic.js"));
        }

        private static void BuildBundleForLeaveRequest(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/leavejs").Include(
                "~/Scripts/Views/Request/leave.request.js",
                "~/Scripts/Views/Request/process.leave-checkinout.byemployee.js",
                "~/Scripts/Views/Request/search.request.js"));
        }

        private static void BuildBundleForCheckInOutRequest(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/empcheckinoutjs").Include(
                "~/Scripts/Views/Request/checkinout.request.js",
                "~/Scripts/Views/Request/process.leave-checkinout.byemployee.js",
                "~/Scripts/Views/Request/search.request.js"));
        }

        private static void BuildBundleForMyTimeSheetReport(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/mytimesheetjs").Include(
                "~/Scripts/Views/Report/report.timesheet.js"));

            bundles.AddAndInclude(
                "~/Content/mytimesheetcss",
                "~/Content/Views/report.timesheet.css");
        }

        private static void BuildBundleForMyCalendar(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/mycalendarjs").Include(
                "~/Scripts/Views/mycalendar.js"));

            bundles.AddAndInclude(
                "~/Content/mycalendarcss",
                "~/Content/Views/mycalendar.css");
        }

        private static void BuildBundleForMemberRequests(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/managerequestsjs").Include(
                "~/Scripts/Libs/jquery.twbsPagination.min.js",
                "~/Scripts/Views/Request/mgr.requests.js",
                "~/Scripts/Views/_employee.search.in.team.js"));
        }

        private static void BuildBundleForRequestsStatistic(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/requeststatisticjs").Include(
                "~/Scripts/Views/Request/leader.request.monitoring.js"));
        }

        private static void BuildBundleForAnnualLeaveBalance(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/annualleavebalancejs").Include(
                "~/Scripts/Views/AnnualLeaveBalance/index.js",
                "~/Scripts/Views/_employee.search.in.company.js"));

            bundles.Add(new ScriptBundle("~/bundles/employeeannualleavebalancejs").Include(
                "~/Scripts/Views/AnnualLeaveBalance/employee.js"));
        }

        private static void BuildBundleForJobTitle(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jobtitlejs").Include(
                "~/Scripts/Views/jobtitle.js"));

            bundles.AddAndInclude(
                "~/Content/jobtitlecss",
                "~/Content/Views/jobtitle.css");
        }

        private static void BuildBundleForAttendanceReport(BundleCollection bundles)
        {
            bundles.AddAndInclude(
                "~/bundles/attendancejs",
                "~/Scripts/Views/Report/report.attendance.js");
        }

        private static void BuildBundleForUserManagement(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/usermanagementjs").Include(
                "~/Scripts/Views/Employee/user.management.js"));
        }

        private static void BuildBundleForLateInEarlyOut(BundleCollection bundles)
        {
            bundles.AddAndInclude(
                "~/bundles/lateinearlyoutsummaryjs",
                "~/Scripts/Views/_employee.search.in.company.js",
                "~/Scripts/Views/Report/report.latein.earlyout.summary.js");
        }

        private static void BuildBundleForLateInEarlyOutDetail(BundleCollection bundles)
        {
            bundles.AddAndInclude(
                "~/bundles/lateinearlyoutdetailjs",
                "~/Scripts/Views/_employee.search.in.company.js",
                "~/Scripts/Views/Report/report.latein.earlyout.detail.js");
        }

        private static void BuildBundleForEditEmployee(BundleCollection bundles)
        {
            bundles.AddAndInclude(
                "~/Content/employeeeditcss",
                "~/Content/Views/employee.edit.css");

            bundles.Add(new ScriptBundle("~/bundles/employeeeditjs").Include(
                "~/Scripts/Libs/birthdaypicker.js",
                "~/Scripts/Views/Employee/employee.edit.js"));
        }

        private static void BuildBundleForTeamManagement(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/teammanagementjs").Include(
                "~/Scripts/Views/team.management.js"));
        }

        private static void BuildBundleForDefaultSchedule(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/defaultschedulejs").Include(
                "~/Scripts/Views/Employee/employee.defaultschedule.js"));
        }

        private static void BuildBundleForMonthSchedule(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/monthschedulejs").Include(
                "~/Scripts/Views/Employee/employee.monthschedule.js"));
        }

        private static void BuildBundleForReportMyTimeRecord(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/mytimerecordjs").Include(
                "~/Scripts/Views/Report/report.mytimerecord.js"));
        }

        private static void BuildBundleForReportTimeRecord(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/timerecordjs").Include(
                "~/Scripts/Views/_employee.search.in.company.js",
                "~/Scripts/Views/Report/report.timerecord.js"));
        }

        private static void BuildBundleForLeaveSummary(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/leavesummaryjs").Include(
             "~/Scripts/Views/_employee.search.in.company.js",
             "~/Scripts/Views/Report/report.leave.summary.js"));
        }

        private static void BuildBundleForDashboard(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/dashboardjs").Include(
                "~/Scripts/Libs/slick.min.js",
                "~/Scripts/Views/Dashboard/employee.dashboard.js",
                ////"~/Scripts/Views/Dashboard/teamrequest.js",
                "~/Scripts/Libs/moment.js",
                "~/Scripts/Libs/fullcalendar.js",
                "~/Scripts/Libs/gcal.js",
                "~/Scripts/Views/Dashboard/calendar.js",
                "~/Scripts/Views/Dashboard/googleAPI.js",
                "~/Scripts/Views/Dashboard/client.js"));

            bundles.AddAndInclude(
                "~/Content/dashboardcss",
                "~/Content/Common/slick.css",
                "~/Content/Views/mydashboard.css",
                "~/Content/Common/fullcalendar.css",
                "~/Content/Views/mycalendar.css");
        }

        private static void BuildBundleForWorkingCanlendar(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/workingcalendarsjs").Include(
                "~/Scripts/Views/Employee/employee.workingcalendars.js",
                "~/Scripts/Views/_employee.search.in.company.js",
                "~/Scripts/Views/fixtable.js"));

            bundles.AddAndInclude(
               "~/Content/workingcalendarscss",
               "~/Content/Views/report.timesheet.css");
        }

        private static void BuildBundleForStaffInfo(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/staffinfojs").Include(
                "~/Scripts/Views/Employee/employee.index.js"));
        }

        private static void BuildBundleForStaffBirthday(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/staffbirthdayjs").Include(
                "~/Scripts/Views/Employee/employee.birthday.js"));

            bundles.AddAndInclude(
                "~/Content/staffbirthdaycss",
                "~/Content/Views/employee.birthday.css");
        }

        private static void BuildBundleForLeaveDeductionDetail(BundleCollection bundles)
        {
            bundles.AddAndInclude(
                "~/bundles/leavedeductiondetailjs",
                "~/Scripts/Views/_employee.search.in.company.js",
                "~/Scripts/Views/Report/report.leave.deduction.detail.js");
        }

        private static void BuildBundleForMyProfile(BundleCollection bundles)
        {
            bundles.AddAndInclude(
                "~/Content/myprofilecss",
                "~/Content/Views/my.profile.css");
        }

        private static void BuildBundleForSimplePagination(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/simplepaginationjs").Include(
                "~/Libs/SimplePagination/jquery.simplePagination.js"));

            bundles.AddAndInclude(
                "~/Content/simplepaginationcss",
                "~/Libs/SimplePagination/simplePagination.css");
        }
    }
}